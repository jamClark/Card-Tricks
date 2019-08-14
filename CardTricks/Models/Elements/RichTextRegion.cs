using CardTricks.Attributes;
using CardTricks.Controls;
using CardTricks.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace CardTricks.Models.Elements
{
    [Serializable]
    [DataContract]
    public class RichTextRegion : BaseElement, ITemplateElement<FlowDocument>
    {
        #region Private Members
        [DataMember(Name="FlowDocument")]
        private string SerialData = "";
        private FlowDocument _Content;
        #endregion

        
        #region Public properties
        [ModelProp(BindingMode = BindingMode.TwoWay, UpdateTrigger = UpdateSourceTrigger.PropertyChanged, BindTarget = BindTarget.Auto)]
        [ContentElementCloneAttribute]
        public FlowDocument Content
        {
            get
            {
                return _Content;
            }

            set
            {
                //BUG ALERT: If 'Content' is a string, this equality check might give us hell.
                if ((_Content == null && value != null) || _Content != value)
                {
                    _Content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }

        public override string ContentToString
        {
            get { return StringFromFlowDoc(_Content); }
        }
        #endregion


        #region Public Methods
        public RichTextRegion(string name, ITemplate parent, IElement inheritsFrom = null) : base(name, parent, inheritsFrom)
        {
            Initialize();
            BindEditProps();
        }

        public RichTextRegion() : this(null, null, null) {}

        private void Initialize()
        {
            //Name = name;
            Width = 1.25 * TemplateUserControl.DPI;
            Height = 1.25 * TemplateUserControl.DPI;
            Content = new FlowDocument();
            BackgroundColor = Colors.Transparent;

            AllowFocus = true;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            //THIS WILL BE QUITE SLOW!!!

            //save the data as XML
            MemoryStream stream = new MemoryStream();
            XamlWriter.Save(_Content, stream);
            stream.Position = 0;

            //convert the XML stream to a string
            StreamReader reader = new StreamReader(stream);
            SerialData = reader.ReadToEnd();

            //Now, SerialData will be 'ready' for writing to the file using the DataContract serializer. Lame huh?
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            //we've used this data already, just nullify it so we don't waste memory
            SerialData = "";
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
            Initialize();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            //now we need to convert that damn string BACK to an XamlStream and then convert that to a FlowDocument. *sigh*
            _Content = XamlReader.Parse(SerialData) as FlowDocument;
            SerialData = "";

            //Again! Like in the constructor, we *must* call this and only *after*
            //all of the data has been recovered/initialized for this element.
            BindEditProps();
            BindInstanceEditProps();

        }

        public override UIElement GenerateLayout()
        {
            if(_Content.Parent != null)
            {
                RichTextBox owner = _Content.Parent as RichTextBox;
                owner.Document = new FlowDocument();//remove ref to this FlowDocument we have on _Content
            }
            SuperRichTextBox Textbox = new SuperRichTextBox();
            Textbox.rtbText.Document = _Content;
            Textbox.rtbText.Width = Double.NaN;
            Textbox.rtbText.Height = Double.NaN;
            Textbox.rtbText.HorizontalAlignment = HorizontalAlignment.Stretch;
            Textbox.rtbText.VerticalAlignment = VerticalAlignment.Stretch;
            Textbox.rtbText.Margin = new Thickness(0); //set margin to 0 to remove double line spacing
            Textbox.Margin = new Thickness(10);
            Textbox.rtbText.AcceptsReturn = true;
            Textbox.rtbText.Name = "TextContent";
            Textbox.rtbText.TextChanged += TextHandler;

            ElementUserControl control = new ElementUserControl(this);
            control.AddChild(Textbox);
            Textbox.DataContext = control.DataContext;
            
            BindLayoutToEditProps(control);
            BindModelHelper(Textbox.rtbText, "BackgroundColor", BindableRichTextBox.BackgroundProperty);
            BindModelHelper(Textbox.rtbText, "BackgroundColor", BindableRichTextBox.BorderBrushProperty);
            BindModelHelper(control, "Content", BindableRichTextBox.DocumentProperty);

            LastLayoutInstance = control;
            control.CanManipulate = !Locked;
            return control;
        }

        public override void HandleManualUpdates()
        {
            //we need to get the info from the local
            //textbox and read it back to our Content property.
            //if(Textbox != null) Content = Textbox.Document;
        }

        private void TextHandler(object sender, TextChangedEventArgs e)
        {
            if (Name.Equals(Card.ElementNameCardBinding)) OnChangeName();
        }
        #endregion


        #region Private Methods
        protected override void DeepClone(PropertyInfo sourceProp, BaseElement sourceElm, PropertyInfo destProp, BaseElement destElm)
        {
            RichTextRegion sourceText = sourceElm as RichTextRegion;
            RichTextRegion destText = destElm as RichTextRegion;
            destText.Content = new FlowDocument();
            AddDocument(sourceText.Content, destText.Content);
            destText.Content.IsOptimalParagraphEnabled = sourceText.Content.IsOptimalParagraphEnabled;
            destText.Content.IsHyphenationEnabled = sourceText.Content.IsHyphenationEnabled;
        }

        /// <summary>
        /// Adds a block to a flowdocument.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        private static void AddBlock(Block from, FlowDocument to)
        {
            
            if (from != null)
            {
                TextRange range = new TextRange(from.ContentStart, from.ContentEnd);
                MemoryStream stream = new MemoryStream();
                System.Windows.Markup.XamlWriter.Save(range, stream);
                range.Save(stream, DataFormats.XamlPackage);
                TextRange textRange2 = new TextRange(to.ContentEnd, to.ContentEnd);
                textRange2.Load(stream, DataFormats.XamlPackage);
            }
        }

        /// <summary>
        /// Adds one flowdocument to another.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        private static void AddDocument(FlowDocument from, FlowDocument to)
        {
            TextRange range = new TextRange(from.ContentStart, from.ContentEnd);
            MemoryStream stream = new MemoryStream();
            System.Windows.Markup.XamlWriter.Save(range, stream);
            range.Save(stream, DataFormats.XamlPackage);
            TextRange range2 = new TextRange(to.ContentEnd, to.ContentEnd);
            range2.Load(stream, DataFormats.XamlPackage);
        }

        /// <summary>
        /// Returns a plain-text string from this element's FlowDocument.
        /// </summary>
        /// <param name="rtb"></param>
        /// <returns></returns>
        string StringFromFlowDoc(FlowDocument doc)
        {
            TextRange textRange = new TextRange(doc.ContentStart, doc.ContentEnd);
            string text = textRange.Text;
            return text.Trim();
        }
        #endregion

    }
}
