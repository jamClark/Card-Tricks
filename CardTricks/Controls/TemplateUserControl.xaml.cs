using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CardTricks.Interfaces;
using CardTricks.Models;
using System.ComponentModel;
using System.Runtime.Serialization;


namespace CardTricks.Controls
{
    /// <summary>
    /// Interaction logic for TemplateControl.xaml
    /// </summary>
    public partial class TemplateUserControl : UserControl, IDesignItemDecorator, INotifyPropertyChanged
    {
        public static readonly string LayoutSurfaceId = "layoutsurfaceContent";

        #region Public Properties
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly RoutedEvent CanManipulateSetEvent =
                                EventManager.RegisterRoutedEvent("CanManipulateSet", RoutingStrategy.Direct,
                                typeof(RoutedEventHandler), typeof(TemplateUserControl));

        public static readonly RoutedEvent CanManipulateUnsetEvent =
                                EventManager.RegisterRoutedEvent("CanManipulateUnset", RoutingStrategy.Direct,
                                typeof(RoutedEventHandler), typeof(TemplateUserControl));

        public event RoutedEventHandler CanManipulateSet
        {
            add { AddHandler(CanManipulateSetEvent, value); }
            remove { RemoveHandler(CanManipulateSetEvent, value); }
        }

        public event RoutedEventHandler CanManipulateUnset
        {
            add { AddHandler(CanManipulateUnsetEvent, value); }
            remove { RemoveHandler(CanManipulateUnsetEvent, value); }
        }

        public Canvas LayoutSurface
        {
            get
            {
                //return this.Template.FindName("canvasContent", this) as Canvas;
                return this.layoutsurfaceContent;
            }
        }

        public static double DPI
        {
            get { return 96.0; }
        }

        public IElement Creator;

        protected bool _CanManipulate;
        public bool CanManipulate
        {
            get { return _CanManipulate; }
            set
            {
                _CanManipulate = value;
                ItemDecorator.IsHitTestVisible = value;
                foreach (UIElement con in layoutsurfaceContent.Children)
                {
                    if (con is ElementUserControl) ((ElementUserControl)con).CanManipulate = value;
                    //else if (con is TemplateUserControl) ((TemplateUserControl)con).CanManipulate = value;
                }
                NotifyPropertyChanged("CanManipulate");
                if(value) RaiseEvent(new RoutedEventArgs(TemplateUserControl.CanManipulateSetEvent));
                else RaiseEvent(new RoutedEventArgs(TemplateUserControl.CanManipulateUnsetEvent));
            }
        }

        #endregion


        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        public TemplateUserControl(IElement creator)
        {
            Creator = creator;
            InitializeComponent();
            this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            this.Arrange(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));
            this.UpdateLayout();

            Canvas.SetTop(this, 0);
            Canvas.SetLeft(this, 0);
            CanManipulate = true;
        }

        public TemplateUserControl() : this(null)
        {
            
        }

        public TemplateUserControl(SerializationInfo info, StreamingContext context)
        {
            
        }

        public void AddChild(UIElement element)
        {
            this.layoutsurfaceContent.Children.Add(element);
        }

        public void RemoveChild(UIElement element)
        {
            this.layoutsurfaceContent.Children.Remove(element);
        }

        public void RegisterMouseClick(MouseButtonEventHandler func)
        {
            this.PreviewMouseDown += func;
            
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        #endregion

        public bool ShowDecorator
        {
            get
            {
                return this.ItemDecorator.ShowDecorator;
            }
            set
            {
                this.ItemDecorator.ShowDecorator = value;
            }
        }
    }
}
