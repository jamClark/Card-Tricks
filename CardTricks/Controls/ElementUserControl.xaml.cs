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
using CardTricks.Cmds;
using System.ComponentModel;
using CardTricks.Windows;


namespace CardTricks.Controls
{
    /// <summary>
    /// Interaction logic for TemplateControl.xaml
    /// </summary>
    public partial class ElementUserControl : UserControl, IDesignItemDecorator
    {
        public static readonly string LayoutSurfaceId = "layoutsurfaceContent";

        #region Public Properties
        public Grid LayoutSurface
        {
            get
            {
                //return this.Template.FindName("canvasContent", this) as Canvas;
                return this.layoutsurfaceContent;
            }
        }

        public static double DPI
        {
            get { return TemplateUserControl.DPI; }
        }

        public BaseElement Creator;

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

        protected bool _CanManipulate;
        public bool CanManipulate
        {
            get { return _CanManipulate; }
            set
            {
                //set the flag
                _CanManipulate = value;
                //ShowDecorator = value; //doesn't work for some reason

                //now we need to set some other values that depend on the state
                //of our manipulation flag.
                if (_CanManipulate)
                {
                    //enable context menu
                    this.ContextMenu = App.Current.Resources["BaseElementContextMenu"] as ContextMenu;
                }
                else
                {
                    //disable context menu
                    this.ClearValue(ContextMenuProperty);

                    //hide decorator
                    ShowDecorator = false;
                }

                //next, we determine what sub-elements get their hit-testing disabled
                if (Creator.AllowFocus)
                {
                    //just manipulation (usually for elements that have textboxes)
                    ItemDecorator.IsHitTestVisible = value;
                    MoveThumb.IsHitTestVisible = value;
                }
                else
                {
                    //everything (usually images or static content)
                    this.IsHitTestVisible = value;
                }
               
                //now we set this flag for all child ElementuserControls (if any)
                foreach (UIElement con in layoutsurfaceContent.Children)
                {
                    if (con is ElementUserControl) ((ElementUserControl)con).CanManipulate = value;
                }

            }
        }

        #endregion


        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        public ElementUserControl(BaseElement creator)
        {
            _CanManipulate = true;
            Creator = creator;
            InitializeComponent();
            this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            this.Arrange(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));
            this.UpdateLayout();

            Canvas.SetTop(this, 0);
            Canvas.SetLeft(this, 0);
            
        }

        public ElementUserControl() : this(null)
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
        #endregion


        #region Commands
        private RelayCommand<object> _RemoveElementCmd;
        public ICommand RemoveElementCmd
        {
            get
            {
                if (_RemoveElementCmd == null)
                    _RemoveElementCmd = new RelayCommand<object>(RemoveElement, CanRemoveElement);
                return _RemoveElementCmd;
            }
        }
        private bool CanRemoveElement(object parameter) { return true; }
        private void RemoveElement(object parameter)
        {
            Creator.Parent.RemoveElement(Creator);
            Creator.TriggerRefreshLayout();
        }


        private RelayCommand<object> _RenameElementCmd;
        public ICommand RenameElementCmd
        {
            get
            {
                if (_RenameElementCmd == null)
                    _RenameElementCmd = new RelayCommand<object>(RenameElement, CanRenameElement);
                return _RenameElementCmd;
            }
        }
        private bool CanRenameElement(object parameter) { return true; }
        private void RenameElement(object parameter)
        {
            //Creator.ChangeID(
            //bring up a window to change the id. once the id is set, confirm it
            RenamerWindow dlg = new RenamerWindow("Rename Element", Creator.Name);
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                //attempt to rename
                try
                {
                    Creator.ChangeID(dlg.NewName);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private RelayCommand<object> _LockCmd;
        public ICommand LockCmd
        {
            get
            {
                if (_LockCmd == null)
                    _LockCmd = new RelayCommand<object>(Lock, CanLock);
                return _LockCmd;
            }
        }
        private bool CanLock(object parameter) { return true; }
        private void Lock(object parameter)
        {
            this.CanManipulate = false;
            Creator.Locked = true;
            Keyboard.ClearFocus();
        }


        private RelayCommand<object> _BringToFrontCmd;
        public ICommand BringToFrontCmd
        {
            get
            {
                if (_BringToFrontCmd == null)
                    _BringToFrontCmd = new RelayCommand<object>(BringToFront, CanBringToFront);
                return _BringToFrontCmd;
            }
        }
        private bool CanBringToFront(object parameter) { return true; }
        private void BringToFront(object parameter)
        {
            //get center of parent template
            ITemplate template = Creator.Parent as ITemplate;
            if (template != null)
            {
                template.MoveElementToFront(Creator);
                Creator.TriggerRefreshLayout();
            }
        }


        private RelayCommand<object> _SendToBackCmd;
        public ICommand SendToBackCmd
        {
            get
            {
                if (_SendToBackCmd == null)
                    _SendToBackCmd = new RelayCommand<object>(SendToBack, CanSendToBack);
                return _SendToBackCmd;
            }
        }
        private bool CanSendToBack(object parameter) { return true; }
        private void SendToBack(object parameter)
        {
            //get center of parent template
            ITemplate template = Creator.Parent as ITemplate;
            if (template != null)
            {
                template.MoveElementToBack(Creator);
                Creator.TriggerRefreshLayout();
            }
        }


        private RelayCommand<object> _MoveUpLayerCmd;
        public ICommand MoveUpLayerCmd
        {
            get
            {
                if (_MoveUpLayerCmd == null)
                    _MoveUpLayerCmd = new RelayCommand<object>(MoveUpLayer, CanMoveUpLayer);
                return _MoveUpLayerCmd;
            }
        }
        private bool CanMoveUpLayer(object parameter) { return true; }
        private void MoveUpLayer(object parameter)
        {
            //get center of parent template
            ITemplate template = Creator.Parent as ITemplate;
            if (template != null)
            {
                template.MoveElementUp(Creator);
                Creator.TriggerRefreshLayout();
            }
        }


        private RelayCommand<object> _MoveDownLayerCmd;
        public ICommand MoveDownLayerCmd
        {
            get
            {
                if (_MoveDownLayerCmd == null)
                    _MoveDownLayerCmd = new RelayCommand<object>(MoveDownLayer, CanMoveDownLayer);
                return _MoveDownLayerCmd;
            }
        }
        private bool CanMoveDownLayer(object parameter) { return true; }
        private void MoveDownLayer(object parameter)
        {
            //get center of parent template
            ITemplate template = Creator.Parent as ITemplate;
            if (template != null)
            {
                template.MoveElementDown(Creator);
                Creator.TriggerRefreshLayout();
            }
        }


        private RelayCommand<object> _PositionCenterCmd;
        public ICommand PositionCenterCmd
        {
            get
            {
                if (_PositionCenterCmd == null)
                    _PositionCenterCmd = new RelayCommand<object>(PositionCenter, CanPositionCenter);
                return _PositionCenterCmd;
            }
        }
        private bool CanPositionCenter(object parameter) { return true; }
        private void PositionCenter(object parameter)
        {
            //get center of parent template
            double xPos = (Creator.Parent.Width / 2) - (Creator.Width / 2);
            double yPos = (Creator.Parent.Height / 2) - (Creator.Height / 2);
            Canvas.SetLeft(this, xPos);
            Canvas.SetTop(this, yPos);
        }


        private RelayCommand<object> _PositionHorLeftToCenterCmd;
        public ICommand PositionHorLeftToCenterCmd
        {
            get
            {
                if (_PositionHorLeftToCenterCmd == null)
                    _PositionHorLeftToCenterCmd = new RelayCommand<object>(PositionHorLeftToCenter, CanPositionHorLeftToCenter);
                return _PositionHorLeftToCenterCmd;
            }
        }
        private bool CanPositionHorLeftToCenter(object parameter) { return true; }
        private void PositionHorLeftToCenter(object parameter)
        {
            //get center of parent template
            double xPos = (Creator.Parent.Width / 2);
            Canvas.SetLeft(this, xPos);
        }


        private RelayCommand<object> _PositionHorLeftToLeftCmd;
        public ICommand PositionHorLeftToLeftCmd
        {
            get
            {
                if (_PositionHorLeftToLeftCmd == null)
                    _PositionHorLeftToLeftCmd = new RelayCommand<object>(PositionHorLeftToLeft, CanPositionHorLeftToLeft);
                return _PositionHorLeftToLeftCmd;
            }
        }
        private bool CanPositionHorLeftToLeft(object parameter) { return true; }
        private void PositionHorLeftToLeft(object parameter)
        {
            Canvas.SetLeft(this, 0);
        }


        private RelayCommand<object> _PositionHorRightToCenterCmd;
        public ICommand PositionHorRightToCenterCmd
        {
            get
            {
                if (_PositionHorRightToCenterCmd == null)
                    _PositionHorRightToCenterCmd = new RelayCommand<object>(PositionHorRightToCenter, CanPositionHorRightToCenter);
                return _PositionHorRightToCenterCmd;
            }
        }
        private bool CanPositionHorRightToCenter(object parameter) { return true; }
        private void PositionHorRightToCenter(object parameter)
        {
            //get center of parent template
            double xPos = (Creator.Parent.Width / 2) - Creator.Width;
            Canvas.SetLeft(this, xPos);
        }


        private RelayCommand<object> _PositionHorRightToRightCmd;
        public ICommand PositionHorRightToRightCmd
        {
            get
            {
                if (_PositionHorRightToRightCmd == null)
                    _PositionHorRightToRightCmd = new RelayCommand<object>(PositionHorRightToRight, CanPositionHorRightToRight);
                return _PositionHorRightToRightCmd;
            }
        }
        private bool CanPositionHorRightToRight(object parameter) { return true; }
        private void PositionHorRightToRight(object parameter)
        {
            Canvas.SetLeft(this, Creator.Parent.Width - Creator.Width);
        }


        private RelayCommand<object> _PositionHorCenterCmd;
        public ICommand PositionHorCenterCmd
        {
            get
            {
                if (_PositionHorCenterCmd == null)
                    _PositionHorCenterCmd = new RelayCommand<object>(PositionHorCenter, CanPositionHorCenter);
                return _PositionHorCenterCmd;
            }
        }
        private bool CanPositionHorCenter(object parameter) { return true; }
        private void PositionHorCenter(object parameter)
        {
            //get center of parent template
            double xPos = (Creator.Parent.Width / 2) - (Creator.Width / 2);
            Canvas.SetLeft(this, xPos);
        }


        private RelayCommand<object> _PositionVertTopToCenterCmd;
        public ICommand PositionVertTopToCenterCmd
        {
            get
            {
                if (_PositionVertTopToCenterCmd == null)
                    _PositionVertTopToCenterCmd = new RelayCommand<object>(PositionVertTopToCenter, CanPositionVertTopToCenter);
                return _PositionVertTopToCenterCmd;
            }
        }
        private bool CanPositionVertTopToCenter(object parameter) { return true; }
        private void PositionVertTopToCenter(object parameter)
        {
            //get center of parent template
            double yPos = (Creator.Parent.Height / 2);
            Canvas.SetTop(this, yPos);
        }


        private RelayCommand<object> _PositionVertTopToTopCmd;
        public ICommand PositionVertTopToTopCmd
        {
            get
            {
                if (_PositionVertTopToTopCmd == null)
                    _PositionVertTopToTopCmd = new RelayCommand<object>(PositionVertTopToTop, CanPositionVertTopToTop);
                return _PositionVertTopToTopCmd;
            }
        }
        private bool CanPositionVertTopToTop(object parameter) { return true; }
        private void PositionVertTopToTop(object parameter)
        {
            Canvas.SetTop(this, 0);
        }


        private RelayCommand<object> _PositionVertBottomToCenterCmd;
        public ICommand PositionVertBottomToCenterCmd
        {
            get
            {
                if (_PositionVertBottomToCenterCmd == null)
                    _PositionVertBottomToCenterCmd = new RelayCommand<object>(PositionVertBottomToCenter, CanPositionVertBottomToCenter);
                return _PositionVertBottomToCenterCmd;
            }
        }
        private bool CanPositionVertBottomToCenter(object parameter) { return true; }
        private void PositionVertBottomToCenter(object parameter)
        {
            //get center of parent template
            double yPos = (Creator.Parent.Height / 2) - Creator.Height;
            Canvas.SetTop(this, yPos);
        }


        private RelayCommand<object> _PositionVertBottomToBottomCmd;
        public ICommand PositionVertBottomToBottomCmd
        {
            get
            {
                if (_PositionVertBottomToBottomCmd == null)
                    _PositionVertBottomToBottomCmd = new RelayCommand<object>(PositionVertBottomToBottom, CanPositionVertBottomToBottom);
                return _PositionVertBottomToBottomCmd;
            }
        }
        private bool CanPositionVertBottomToBottom(object parameter) { return true; }
        private void PositionVertBottomToBottom(object parameter)
        {
            Canvas.SetTop(this, Creator.Parent.Height - Creator.Height);
        }

        private RelayCommand<object> _PositionVertCenterCmd;
        public ICommand PositionVertCenterCmd
        {
            get
            {
                if (_PositionVertCenterCmd == null)
                    _PositionVertCenterCmd = new RelayCommand<object>(PositionVertCenter, CanPositionVertCenter);
                return _PositionVertCenterCmd;
            }
        }
        private bool CanPositionVertCenter(object parameter) { return true; }
        private void PositionVertCenter(object parameter)
        {
            //get center of parent template
            double yPos = (Creator.Parent.Height / 2) - (Creator.Height / 2);
            Canvas.SetTop(this, yPos);
        }
        #endregion

    }
}
