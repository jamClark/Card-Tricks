using CardTricks.Attributes;
using CardTricks.Controls;
using CardTricks.Converters;
using CardTricks.Interfaces;
using CardTricks.Utils;
using CardTricks.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CardTricks.Models.Elements
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ImageListElement : BaseElement, ITemplateElement<BitmapWrapper>
    {
        #region Private Members
        private BitmapWrapper _Content;
        private Color _Tint;
        private string _SelectedFile;
        private IList<string> _ImageFiles;

       #endregion

        
        #region Public Properties
        
        //BUG:!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //This doesn't have a way of getting updated by the control's list
        [ShallowElementCloneAttribute]
        [ReconcilableProp]
        public IList<string> ImagesFiles
        {
            get {return _ImageFiles;}
            set
            {
                if (_ImageFiles != value)
                {
                    _ImageFiles = value;
                    NotifyPropertyChanged("ImageFiles");
                }
            }
        }

        //BUG ALERT: This should be attached to the 'ImageFiles' property but for some reason we can't make it OneWayToSource if we do that.
        [EditProp(
            "Images",
            typeof(ImageListBuilder),
            "SelectedFileProperty",
            LabelWidth = 80,
            BindingMode = BindingMode.OneWayToSource,
            VerticalStacking = true,
            ItemsSourcePath="ImageFiles"
           )]
        [InstanceEditProp(
            "Images",
            typeof(ComboBox),
            LabelWidth = 80,
            BindingMode = BindingMode.OneWayToSource,
            VerticalStacking = true,
            ItemsSourcePath = "ImageFiles",
            SelectedValuePath = "SelectedFile"
            )]
        [ShallowElementCloneAttribute]
        [ReconcilableProp(Override=true)]
        public string SelectedFile
        {
            get { return _SelectedFile; }
            set
            {
                if (_SelectedFile == null && value != null)
                {
                    _SelectedFile = value;
                    NotifyPropertyChanged("SelectedFile");
                    Content.FileName = value;
                }
                else if (_SelectedFile != null && !_SelectedFile.Equals(value))
                {
                    _SelectedFile = value;
                    NotifyPropertyChanged("SelectedFile");
                    _Content.FileName = value;
                    //if (_Content.Image != null)
                    //{
                    //    Width = ((BitmapImage)_Content.Image).Width;
                    //    Height = ((BitmapImage)_Content.Image).Height;
                    //}
                    NotifyPropertyChanged("Content");
                }
            }
        }

         
        [ModelProp(BindingMode = BindingMode.OneWay, BindTarget = BindTarget.Auto, Converter=typeof(BitmapWrapperToBitmapConverter))]
        [ShallowElementCloneAttribute]
        public BitmapWrapper Content
        {
            get { return _Content; }
            set
            {
                if (_Content != null && value != null && _Content.FileName != null)
                {
                    if (!_Content.FileName.Equals(value.FileName))
                    {
                        _Content.FileName = value.FileName;
                        
                    }
                }
                else
                {
                    _Content = value;
                }
                NotifyPropertyChanged("Content");
            }
        }


        [EditProp("Tint", typeof(ColorPicker), "SelectedColorAdvProperty", Width = 150, LabelWidth = 80)]
        [ModelProp(BindingMode = BindingMode.TwoWay, Converter = typeof(ColorToSolidColorBrushConverter))]
        [ShallowElementCloneAttribute]
        [ReconcilableProp]
        public Color Tint
        {
            get { return _Tint; }
            set
            {
                if ((_Tint == null && value != null) || _Tint != value)
                {
                    _Tint = value;
                    NotifyPropertyChanged("Tint");
                }
            }
        }

        //Override this so we can remove attributes.
        public override Color BackgroundColor
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

        public override string ContentToString
        {
            get { return _Content.FileName; }
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <param name="inheritsFrom"></param>
        public ImageListElement(string name, ITemplate parent, IElement inheritsFrom = null) : base(name, parent, inheritsFrom)
        {
            Width = 100;
            Height = 100;
            Content = null;
            _BackgroundColor = Colors.Transparent;
            _Content = new BitmapWrapper(MainWindow.ImagesDirectory);
            _Tint = Colors.Transparent;
            _ImageFiles = new List<string>();
            BindEditProps();
        }

        /// <summary>
        /// This is needed so that cards can
        /// clone templates using reflection.
        /// </summary>
        public ImageListElement() : this(null, null, null) {}

        /// <summary>
        /// Extends baseclass implmentation to add custom controls.
        /// </summary>
        protected override void BindEditProps()
        {
            base.BindEditProps();

            //Create a button that handles getting the image filename
            //ImageListBuilder refImageList = new ImageListBuilder();
            
            //StackPanel stack = new StackPanel();
            //stack.Orientation = Orientation.Horizontal;
            //stack.FlowDirection = FlowDirection.LeftToRight;
            //stack.Margin = new Thickness(2);
            //stack.HorizontalAlignment = HorizontalAlignment.Left;
            //stack.Children.Add(refImageList);
            
            //EditableProperties.Add(stack as UIElement);
        }

        /// <summary>
        /// Implementation of this element's layout rendering.
        /// </summary>
        /// <returns></returns>
        public override UIElement GenerateLayout()
        {
            Image image = new Image();
            image.Width = Double.NaN;
            image.Height = Double.NaN;
            image.HorizontalAlignment = HorizontalAlignment.Stretch;
            image.VerticalAlignment = VerticalAlignment.Stretch;
            image.Margin = new Thickness(0);
            image.IsHitTestVisible = false;
            image.Stretch = Stretch.Fill;
            //TODO: make an EditProp to set the Stretch style

            Rectangle rect = new Rectangle();
            rect.Width = Double.NaN;
            rect.Height = Double.NaN;
            rect.HorizontalAlignment = HorizontalAlignment.Stretch;
            rect.VerticalAlignment = VerticalAlignment.Stretch;
            rect.Margin = new Thickness(0);
            rect.IsHitTestVisible = false;
            rect.Stretch = Stretch.Fill;

            
            //We must always always always wrap our content inside one of these.
            ElementUserControl control = new ElementUserControl(this);
            control.AddChild(image);
            control.AddChild(rect);
            
            //call this to bind the model to the layout
            BindLayoutToEditProps(control);
            BindModelHelper(image, "Content", Image.SourceProperty);
            BindModelHelper(rect, "Tint", Rectangle.FillProperty);
            
            //This lets the element know what layout was generated last and
            //ishow the designer knows to hilight newly created elements.
            LastLayoutInstance = control;
            control.Background = null;
            control.CanManipulate = !Locked;

            return control;
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Event handler for loading a source image when
        /// the Edit Property Panel button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChooseImageButton(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "Image Files PNG|*.png;*.tif;*tiff"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                string full = dlg.FileName;
                string file = full.Substring(full.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
                file = file.Substring(1, file.Length - 1);

                if (!Directory.Exists(MainWindow.ImagesDirectory)) Directory.CreateDirectory(MainWindow.ImagesDirectory);
                try { File.Copy(dlg.FileName, System.IO.Path.Combine(MainWindow.ImagesDirectory, file), true); }
                catch (Exception exception)
                {
                    MessageBox.Show("Error: " + exception.Message +
                        "\n\nFROM: " + dlg.FileName +
                        "\n\nTO: " + System.IO.Path.Combine(MainWindow.ImagesDirectory, file));
                    return;
                }
                _Content.FileName = file;
                if (_Content.Image != null)
                {
                    Width = ((BitmapImage)_Content.Image).Width;
                    Height = ((BitmapImage)_Content.Image).Height;
                }

                
                NotifyPropertyChanged("Content");
                
            }
        }
        #endregion
    }
}
