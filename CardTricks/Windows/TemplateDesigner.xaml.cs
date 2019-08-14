using CardTricks.Cmds;
using CardTricks.Controls;
using CardTricks.Interfaces;
using CardTricks.Models;
using CardTricks.Models.Elements;
using CardTricks.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;



namespace CardTricks.Windows
{
    /// <summary>
    /// Interaction logic for TemplateDesigner.xaml
    /// </summary>
    public partial class TemplateDesigner : Window, INotifyPropertyChanged
    {
        #region Private Members
        MainWindow MainWindow;
        Workspace _Workspace;
        ITemplate _ActiveTemplate;
        IElement _SelectedElement; //this can be the template or an element of the template

        IDesignItemDecorator SelectedDecorator;
        Point WorkspaceMousePosition;
        Point TemplateMousePosition;
        #endregion


        #region Public Methods
        public TemplateDesigner(Workspace workspace, MainWindow mainWindow)
        {
            this.MainWindow = mainWindow;
            _Workspace = workspace;
            InitializeComponent();

            Unloaded += new RoutedEventHandler(TemplateDesigner_Unloaded);

            //we must go through all elements of all templates
            //an re-wire there RefeshLayout event
            foreach (ITemplate template in _Workspace.Game.Templates)
            {
                foreach (IElement element in template.Children)
                {
                    element.RefreshLayout += new RefreshLayoutEvent(OnRefreshLayout);
                }
            }
            
        }

        #endregion

        
        #region Public Properties
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public ITemplate ActiveTemplate
        {
            get { return _ActiveTemplate; }
            set
            {
                //save some data on the old template before switching
                if (_ActiveTemplate != null)
                {
                    _ActiveTemplate.TriggerManualUpdates();
                }

                if (_ActiveTemplate != value)
                {
                    RestoreLayoutView(value);

                    _ActiveTemplate = value;
                    SelectedElement = _ActiveTemplate; //triggers another property change event
                    
                    //TODO: restore the template layout view of the new template
                    NotifyPropertyChanged("ActiveTemplate");
                }
            }
        }

        public IElement SelectedElement
        {
            get { return _SelectedElement; }
            set
            {
                if (_SelectedElement != value)
                {
                    _SelectedElement = value;
                    NotifyPropertyChanged("SelectedElement");
                    NotifyPropertyChanged("EditorPanelControls");
                }
            }
        }

        public ObservableCollection<UIElement> EditorPanelControls
        {
            get 
            {
                if (_SelectedElement != null)
                    return new ObservableCollection<UIElement>(_SelectedElement.EditableProperties);
                else return null;
            }
        }

        public ObservableCollection<ITemplate> ObservableTemplates
        {
            get
            {
                return _Workspace.Game.ObservableTemplates;
                
            }
        }

        public void OnRefreshLayout()
        {
            RestoreLayoutView(ActiveTemplate);
        }

        public Workspace Workspace { get {return _Workspace;}}
        public Game Game { get { return _Workspace.Game; } }
        #endregion


        #region Command Properties
        private CommandWrapper _NewTemplateCmd;
        public ICommand NewTemplateCmd
        {
            get
            {
                if (_NewTemplateCmd == null)
                    _NewTemplateCmd = new CommandWrapper(param => this.NewTemplate(null), param => this.CanNewTemplate(null));
                return _NewTemplateCmd;
            }
        }

        private RelayCommand<object> _AddElementCmd;
        public ICommand AddElementCmd
        {
            get
            {
                if (_AddElementCmd == null)
                    _AddElementCmd = new RelayCommand<object>(AddElement, CanAddElement);
                return _AddElementCmd;
            }
        }

        private RelayCommand<object> _UnlockElementsCmd;
        public ICommand UnlockElementsCmd
        {
            get
            {
                if (_UnlockElementsCmd == null)
                    _UnlockElementsCmd = new RelayCommand<object>(UnlockElements, CanUnlockElements);
                return _UnlockElementsCmd;
            }
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Clears the active template and removes all related UIElements
        /// from the main canvas of this window.
        /// </summary>
        private void ClearDesignerWorkspace()
        {
            foreach (UIElement elm in canvasWorkspace.Children)
            {
                elm.PreviewMouseUp -= elementsMouseClick;
            }
            canvasWorkspace.Children.Clear();
            
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RestoreLayoutView(ITemplate template)
        {
            ClearDesignerWorkspace();

            if (template == null) return;

            //set active template and create a control to represent it.
            TemplateUserControl templateLayout = template.GenerateLayout() as TemplateUserControl;
            if (templateLayout != null)
            {
                //measure, size, and arrage the element
                templateLayout.Width = template.Width;
                templateLayout.Height = template.Height;

                templateLayout.Measure(new Size(canvasWorkspace.ActualWidth, canvasWorkspace.ActualHeight));
                templateLayout.Arrange(new Rect(
                    (canvasWorkspace.ActualWidth / 2) - (templateLayout.ActualWidth / 2),
                    (canvasWorkspace.ActualHeight / 2) - (templateLayout.ActualHeight / 2),
                    templateLayout.Width,
                    templateLayout.Height));
                templateLayout.UpdateLayout();


                this.canvasWorkspace.Children.Add(templateLayout);
                templateLayout.PreviewMouseDown += elementsMouseClick;
            }
        }

        private void TemplateDesigner_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (ITemplate template in _Workspace.Game.Templates)
            {
                foreach (IElement element in template.Children)
                {
                    element.RefreshLayout -= OnRefreshLayout;
                }
            }

            //this is to update the main menu with the newest template list
            if (MainWindow != null) MainWindow.UpdateLayout();
        }
        #endregion


        #region Event Methods
        //*** MOUSE AND KEYBOARD EVENTS ***//
        private void textboxKeyEnter(object sender, KeyboardEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Enter))
            {
                textboxLostFocus(sender, null); //accept
            }
            if (e.KeyboardDevice.IsKeyDown(Key.Escape))
            {
                textboxLostFocus(null, null); //cancel
            }
        }

        private void textboxLostFocus(object sender, RoutedEventArgs e)
        {
            //if (SelectedSet == null) return;
            //if (sender != null) SelectedSet.Name = this.textboxRenamer.Text;
            //NotifyPropertyChanged("CardSets");
            //NotifyPropertyChanged("ObservableCardSets");

            //SelectedSet = null;
            //this.textboxRenamer.Visibility = System.Windows.Visibility.Hidden;
            //this.textboxRenamer.IsEnabled = false;
            MessageBox.Show("Textbox lost focus");
            Keyboard.ClearFocus();
        }

        private void listboxitemTemplatesMouseDown(Object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            if (item != null)
            {
                item.Focus();
            }
        }

        private void listboxitemTemplatesOnSelecteItem(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null)
            {
                ActiveTemplate = item.DataContext as ITemplate;
                SelectedElement = item.DataContext as IElement;
            }
        }

        private List<FrameworkElement> HitList = new List<FrameworkElement>();
        private void elementsMouseClick(object sender, MouseEventArgs e)
        {
            Keyboard.ClearFocus();

            WorkspaceMousePosition = e.GetPosition(canvasWorkspace);
            TemplateMousePosition = e.GetPosition((UIElement)sender);
            HitList.Clear();
            VisualTreeHelper.HitTest(canvasWorkspace, 
                                null, 
                                new HitTestResultCallback(OnHitResults), 
                                new PointHitTestParameters(WorkspaceMousePosition));

            UserControl parent = null;
            if(HitList.Count > 0)
            {
                foreach (FrameworkElement result in HitList)
                {
                    if (result.Name.Equals(TemplateUserControl.LayoutSurfaceId))
                    {
                        ContentControl content = result.Parent as ContentControl;
                        parent = content.Parent as UserControl;
                        //check if is element and can be manipulated, if not pass right through it
                        if (parent is ElementUserControl && !((ElementUserControl)parent).CanManipulate)
                        {
                            parent = null;
                            continue;
                        }
                        break;
                    }
                }
            }

            if (SelectedDecorator != null) SelectedDecorator.ShowDecorator = false;
            SelectedElement = null;

            //find out what we hit and hi-light it if applicable.
            if (parent != null)
            {
                TemplateUserControl temp = parent as TemplateUserControl;
                if (temp == null)
                {
                    ElementUserControl elm = parent as ElementUserControl;
                    if (elm != null)
                    {
                        if (!elm.CanManipulate) return;
                        SelectedElement = elm.Creator;
                        SelectedDecorator = elm;
                    }
                }
                else
                {
                    if (!temp.CanManipulate) return;
                    SelectedElement = temp.Creator;
                    SelectedDecorator = temp;
                }

                SelectedDecorator.ShowDecorator = true;
            }
        }

        private HitTestResultBehavior OnHitResults(HitTestResult result)
        {
            FrameworkElement elm = result.VisualHit as FrameworkElement;
            if(elm != null) HitList.Add(elm);
            return HitTestResultBehavior.Continue;
        }

        private HitTestFilterBehavior FilterHits(DependencyObject o)
        {
            FrameworkElement element = o as FrameworkElement;
            return HitTestFilterBehavior.Continue;
        }

        //*** MENU COMMANDS ***//
        private bool CanNewTemplate(object parameter)
        {
            return true;
        }

        private bool CanAddElement(object parameter)
        {
            return true;
        }

        private bool CanUnlockElements(object parameter) { return true; }

        /// <summary>
        /// Removes currently active template from the workspace and instantiates a fresh new one.
        /// </summary>
        /// <param name="parameter"></param>
        private void NewTemplate(object parameter)
        {
            ClearDesignerWorkspace();
            
            Game game = _Workspace.Game;
            ITemplate template = null;
            try
            {
                template = game.AddTemplate(game.GetDefaultTemplateName());
                template.XPos = 0.25 * TemplateUserControl.DPI;
                template.YPos = 0.25 * TemplateUserControl.DPI;
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            //this implicitly creates the layout view of the template
            ActiveTemplate = template;
            SelectedDecorator = template as IDesignItemDecorator;
        }

        /// <summary>
        /// Adds a new element to the currently active template.
        /// </summary>
        /// <param name="parameter"></param>
        private void AddElement(object parameter)
        {
            if (ActiveTemplate == null) return;
            string name = parameter as string;

            BaseElement element = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(name) as BaseElement;
            if (element != null)
            {
                element.XPos = TemplateMousePosition.X - (element.Width/2);
                element.YPos = TemplateMousePosition.Y - (element.Height/2);
                element.RefreshLayout += new RefreshLayoutEvent(OnRefreshLayout);
                ActiveTemplate.AddElement(element);
                SelectedElement = element;
                RestoreLayoutView(ActiveTemplate);
                
                if (element.LastLayoutInstance != null)
                {
                    SelectedDecorator = element.LastLayoutInstance as IDesignItemDecorator;
                    ((ElementUserControl)element.LastLayoutInstance).CanManipulate = true;
                    ((ElementUserControl)element.LastLayoutInstance).ShowDecorator = true;
                    
                }

                
            }
            else
            {
                MessageBox.Show("There was an error while creating a template of the type '" + parameter + "'.");
            }
        }

        /// <summary>
        /// Unlocks all previous locked elements.
        /// </summary>
        /// <param name="parameter"></param>
        private void UnlockElements(object parameter)
        {
            if(ActiveTemplate == null || ActiveTemplate.LastLayoutInstance == null) return;

            TemplateUserControl templateControl = ActiveTemplate.LastLayoutInstance as TemplateUserControl;
            if (templateControl != null)
            {
                templateControl.CanManipulate = true;
                foreach (UIElement element in templateControl.layoutsurfaceContent.Children)
                {
                    ElementUserControl elm = element as ElementUserControl;
                    if (elm != null)
                    {
                        elm.Creator.Locked = false;
                        elm.CanManipulate = true;
                    }
                }
            }
        }
        #endregion
    }
}
