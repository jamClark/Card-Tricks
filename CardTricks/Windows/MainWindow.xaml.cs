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
using System.Windows.Shapes;

using CardTricks.Interfaces;
using CardTricks.Models;
using CardTricks.Cmds;
using System.ComponentModel;
using System.Collections.ObjectModel;
using CardTricks.Controls;
using System.Reflection;
using System.Windows.Controls.Primitives;
using CardTricks.Utils;
using System.Windows.Threading;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace CardTricks.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Private Members
        Workspace _Workspace;
        ICardModel _ActiveCard;
        ICardDeckModel _ActiveDeck;
        #endregion


        #region Events and Commands
        public event PropertyChangedEventHandler PropertyChanged;

        private CommandWrapper _NewGameCmd;
        public ICommand NewGameCmd
        {
            get
            {
                if (_NewGameCmd == null)
                    _NewGameCmd = new CommandWrapper(param => this.NewGame(null), param => this.CanNewGame(null));
                return _NewGameCmd;
            }
        }

        private CommandWrapper _OpenGameCmd;
        public ICommand OpenGameCmd
        {
            get
            {
                if (_OpenGameCmd == null)
                    _OpenGameCmd = new CommandWrapper(param => this.OpenGame(null), param => this.CanOpenGame(null));
                return _OpenGameCmd;
            }
        }

        private RelayCommand<object> _SaveGameCmd;
        public ICommand SaveGameCmd
        {
            get
            {
                if (_SaveGameCmd == null)
                    _SaveGameCmd = new RelayCommand<object>(SaveGame, CanSaveGame);
                return _SaveGameCmd;
            }
        }

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

        private CommandWrapper _CalculatorWindow;
        public ICommand CalculatorWindow
        {
            get
            {
                if (_CalculatorWindow == null)
                    _CalculatorWindow = new CommandWrapper(param => this.OpenCalculator(null), param => this.CanOpenCalculator(null));
                return _CalculatorWindow;
            }
        }

        private CommandWrapper _NewSetCmd;
        public ICommand NewSetCmd
        {
            get
            {
                if (_NewSetCmd == null)
                    _NewSetCmd = new CommandWrapper(param => this.NewSet(null), param => this.CanNewSet(null));
                return _NewSetCmd;
            }
        }

        private CommandWrapper _NewDeckCmd;
        public ICommand NewDeckCmd
        {
            get
            {
                if (_NewDeckCmd == null)
                    _NewDeckCmd = new CommandWrapper(param => this.NewDeck(null), param => this.CanNewDeck(null));
                return _NewDeckCmd;
            }
        }

        private RelayCommand<object> _NewCardCmd;
        public ICommand NewCardCmd
        {
            get
            {
                if (_NewCardCmd == null)
                    _NewCardCmd = new RelayCommand<object>(NewCard, CanNewCard);
                return _NewCardCmd;
            }
        }

        private RelayCommand<object> _CenterCardViewCmd;
        public ICommand CenterCardViewCmd
        {
            get
            {
                if (_CenterCardViewCmd == null)
                    _CenterCardViewCmd = new RelayCommand<object>(CenterCardView, CanCenterCardView);
                return _CenterCardViewCmd;
            }
        }

        private RelayCommand<object> _ReconcileCardCmd;
        public ICommand ReconcileCardCmd
        {
            get
            {
                if (_ReconcileCardCmd == null)
                    _ReconcileCardCmd = new RelayCommand<object>(ReconcileCard, CanReconcileCard);
                return _ReconcileCardCmd;
            }
        }

        private RelayCommand<object> _ReconcileSetCmd;
        public ICommand ReconcileSetCmd
        {
            get
            {
                if (_ReconcileSetCmd == null)
                    _ReconcileSetCmd = new RelayCommand<object>(ReconcileSet, CanReconcileSet);
                return _ReconcileSetCmd;
            }
        }

        private RelayCommand<object> _LinkCopyCardCmd;
        public ICommand LinkCopyCardCmd
        {
            get
            {
                if (_LinkCopyCardCmd == null)
                    _LinkCopyCardCmd = new RelayCommand<object>(LinkCopyCard, CanLinkCopyCard);
                return _LinkCopyCardCmd;
            }
        }

        private RelayCommand<object> _UniqueCopyCardCmd;
        public ICommand UniqueCopyCardCmd
        {
            get
            {
                if (_UniqueCopyCardCmd == null)
                    _UniqueCopyCardCmd = new RelayCommand<object>(UniqueCopyCard, CanUniqueCopyCard);
                return _UniqueCopyCardCmd;
            }
        }
        #endregion


        #region Public Properties
        public bool ValidWorkspace
        {
            get { return (_Workspace != null); }
        }

        public ICardModel ActiveCard
        {
            get { return _ActiveCard; }
            set
            {
                if (_ActiveCard != value)
                {
                    _ActiveCard = value;
                }
                RestoreLayoutView(_ActiveCard);
                NotifyPropertyChanged("ActiveCard");
                NotifyPropertyChanged("ObservableSets");
                NotifyPropertyChanged("EditorPanelControls");
            }
        }

        public Workspace Workspace
        {
            get { return _Workspace; }
            set
            {
                if (_Workspace != value)
                {
                    _Workspace = value;
                    NotifyPropertyChanged("Workspace");
                }
            }
        }

        public ObservableCollection<ICardSetViewItem> ObservableSets
        {
            get
            {
                if (_Workspace == null || _Workspace.Game == null) return null;
                return _Workspace.Game.ObservableSets; 
            }
        }

        public ObservableCollection<ICardDeckViewItem> ObservableDecks
        {
            get
            {
                if (_Workspace == null || _Workspace.Game == null) return null;
                return _Workspace.Game.ObservableDecks;
            }
        }

        public ObservableCollection<ITemplate> ObservableTemplates
        {
            get
            {
                if (_Workspace == null || _Workspace.Game == null) return null;
                //just in case something changed, trigger HasTemplates refresh
                return _Workspace.Game.ObservableTemplates;
            }
        }

        public ObservableCollection<UIElement> EditorPanelControls
        {
            get
            {
                if (_ActiveCard != null && _ActiveCard.Template != null)
                {
                    return new ObservableCollection<UIElement>(_ActiveCard.Template.GetHierachyOfInstanceProperties());
                }
                    
                else return null;
            }
        }

        public IList<MenuItem> TemplatesMenuList
        {
            get
            {
                IList<MenuItem> list = new List<MenuItem>();
                if (_Workspace == null || _Workspace.Game == null)
                {
                    MenuItem item = new MenuItem();
                    item.Header = "No Templates Available";
                    list.Add(item);
                }
                else
                {
                    foreach (ITemplate t in ObservableTemplates)
                    {
                        MenuItem item = new MenuItem();
                        item.Header = "Add Card - " + t.Name;
                        list.Add(item);
                    }
                }
                return list;
            }
        }

        public bool HasTemplates
        {
            get 
            {
                if (_Workspace == null || _Workspace.Game == null) return false;
                return _Workspace.Game.HasTemplates; 
            }
        }

        public static string RootDirectory { get { return System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); } }

        //yuck! Singleton-like stuff
        private static Workspace StaticWorkspace = null;
        public static string ImagesDirectory 
        { 
            //get { return RootDirectory + System.IO.Path.DirectorySeparatorChar + "Images"; }
            get
            {
                if (StaticWorkspace != null) return StaticWorkspace.ImageFolder;
                return null;
            }
        }
        #endregion


        #region Public Methods
        public MainWindow()
        {
            InitializeComponent();

            //_Workspace = new Workspace("Card Tricks Demo", "Card Gamee TCG");
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        private void RestoreLayoutView(ICardModel card)
        {
            ClearDesignerWorkspace();
            if (card == null) return;
            ITemplate template = card.Template;
            if (template == null) return;

            //set active template and create a control to represent it.
            TemplateUserControl templateLayout = template.GenerateLayout() as TemplateUserControl;
            templateLayout.CanManipulate = false;
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

        /// <summary>
        /// We are perfomring hit testing on all elements of the template.
        /// This way we can filter out Textboxes and allow them to still
        /// be clicked even when the parent ElementUserControl has been disabled
        /// for hit testing.
        /// </summary>
        private List<FrameworkElement> HitList = new List<FrameworkElement>();
        private void elementsMouseClick(object sender, MouseEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private HitTestResultBehavior OnHitResults(HitTestResult result)
        {
            FrameworkElement elm = result.VisualHit as FrameworkElement;
            if (elm != null) HitList.Add(elm);
            return HitTestResultBehavior.Continue;
        }

        private HitTestFilterBehavior FilterHits(DependencyObject o)
        {
            FrameworkElement element = o as FrameworkElement;
            return HitTestFilterBehavior.Continue;
        }

        /// <summary>
        /// Creates a new workspace. This also clears out the old workspace and notifies
        /// all controls that they need to refresh with the default 'empty' data.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="gameName"></param>
        /// <param name="rootFolder"></param>
        private void CreateNewWorkspace(string projectName, string gameName, string rootFolder)
        {
            _Workspace = new Workspace(projectName, gameName, rootFolder);
            this.Title = "Card Tricks - " + projectName;

            //HACK ALERT: I'm using a singleton-like process here so that elements can have
            //bitmap wrappers that can get to the project's image directory.
            StaticWorkspace = _Workspace;
            
            ActiveCard = null;
            NotifyPropertyChanged("ActiveCard");
            NotifyPropertyChanged("Workspace");
            NotifyPropertyChanged("ObservableSets");
            NotifyPropertyChanged("ObservableDecks");
            NotifyPropertyChanged("ObservableTemplates");
            NotifyPropertyChanged("EditorPanelControls");
            NotifyPropertyChanged("TemplatesMenuList");
            NotifyPropertyChanged("HasTemplates");
            ClearDesignerWorkspace();
        }
        #endregion


        #region Event Handlers
        private void treeSetsMouseDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null) item.Focus();
        }

        private void treeviewSetsMouseUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null) item.Focus();
        }

        private void setstreeOnItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            ITreeViewItem model = item.Header as ITreeViewItem;
            //item.Focus();
            if (model != null)
            {
                if (model.IsLeaf)
                {
                    this.ActiveCard = model as ICardModel;
                    model.IsExpanded = true;
                    CenterCardView(null);
                }
                
            }

        }

        /// <summary>
        /// Handles the double-click event for the Sets tree items.
        /// NOTE: This will trigger twice, once for the item and once
        /// for the tree. We must stop it from triggering the second time
        /// by manually checking for the selected item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        bool DoubleEvent = false;
        private void treeSetsDoubleClick(object sender, MouseButtonEventArgs args)
        {
            if (!DoubleEvent)
            {
                DoubleEvent = true;
                if (_ActiveDeck == null)
                {
                    MessageBox.Show("Please select a deck first.");
                    return;
                }
                if (_ActiveDeck != null && ActiveCard != null)
                {
                    _ActiveDeck.RegisterCard(ActiveCard);
                    try
                    {
                        NotifyPropertyChanged("ObservableDecks");
                    }
                    catch (InvalidOperationException e)
                    {
                        treeviewDecks.SetValue(TreeView.SelectedItemProperty, null);
                        treeviewSets.SetValue(TreeView.SelectedItemProperty, null);
                        //NotifyPropertyChanged("ObservableDecks");
                    }
                }

            }
            else DoubleEvent = false;

        }

        private void deckstreeOnItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            ITreeViewItem model = item.Header as ITreeViewItem;
            //item.Focus();
            if (model != null)
            {
                if (model.IsLeaf)
                {
                    this.ActiveCard = model as ICardModel;
                    _ActiveDeck = TreeHelper.InferDeck(item);
                    CenterCardView(null);
                }
                else _ActiveDeck = model as ICardDeckViewItem;

            }

        }
        #endregion


        #region Command Handlers
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool CanNewGame(object parameter) { return true; }

        private void NewGame(object parameter)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.Description = "Select your workspace's root folder.";
            dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog(this) == true)
            {
                string full = dialog.SelectedPath;
                string file = full.Substring(full.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
                file = file.Substring(1, file.Length - 1);

                ProjectNameWindow namer = new ProjectNameWindow("Name your project.", file, file);
                namer.ShowDialog();

                //TODO: we'll need to add a promp for the project and Game names but for now we'll just use the folder
                CreateNewWorkspace(namer.ProjectName, namer.GameName, dialog.SelectedPath);
            }
        }

        private bool CanSaveGame(object parameter) { return ValidWorkspace; }

        private void SaveGame(object parameter)
        {
            string fileName = System.IO.Path.Combine(_Workspace.RootFolder, _Workspace.ProjectName) + ".ctp";
            var settings = new XmlWriterSettings { Indent = true };
            
            using (XmlWriter writer = XmlWriter.Create(fileName, settings))
            {
                //CTSerializer.Write(writer, _Workspace, "Workspace");
                DataContractSerializerSettings serSettings = new DataContractSerializerSettings();
                serSettings.PreserveObjectReferences = true;
                DataContractSerializer serializer = new DataContractSerializer(typeof(Workspace), serSettings);
                serializer.WriteObject(writer, _Workspace);

                writer.Close();
            }
            
            /*
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, _Workspace);
            stream.Close();
            */
        }

        private bool CanOpenGame(object parameter) { return true; }

        private void OpenGame(object parameter)
        {
            Ookii.Dialogs.Wpf.VistaOpenFileDialog dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            //dialog.Description = "Select your workspace's root folder.";
            dialog.DefaultExt = "ctp";
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            dialog.Title = "Select a Project to Open";
            //dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog(this) == true)
            {
                //string fileName = System.IO.Path.Combine(_Workspace.RootFolder, _Workspace.ProjectName) + ".ctp";
                string fileName = dialog.FileName;
                //var settings = new XmlReaderSettings;();// { Indent = true };

                using (XmlReader reader = XmlReader.Create(fileName))
                {
                    //CTSerializer.Write(writer, _Workspace, "Workspace");
                    DataContractSerializerSettings serSettings = new DataContractSerializerSettings();
                    serSettings.PreserveObjectReferences = true;
                    DataContractSerializer serializer = new DataContractSerializer(typeof(Workspace), serSettings);
                    //serializer.WriteObject(writer, _Workspace);
                    _Workspace = serializer.ReadObject(reader) as Workspace;
                    if (_Workspace == null)
                    {
                        MessageBox.Show("The resulting workspace within the file '" + fileName + "' was invalid.");
                    }
                    reader.Close();
                }

                int i = fileName.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                //string dir = fileName.Substring(System.IO.Path.DirectorySeparatorChar);
                //int i = fileName.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                //dir = dir.Replace(dir, "");
                string dir = fileName.Remove(i);

                _Workspace.ProvideWorkspaceRoot(dir);
                this.Title = "Card Tricks - " + this._Workspace.ProjectName;
                ActiveCard = null;
                NotifyPropertyChanged("ActiveCard");
                NotifyPropertyChanged("Workspace");
                NotifyPropertyChanged("ObservableSets");
                NotifyPropertyChanged("ObservableDecks");
                NotifyPropertyChanged("ObservableTemplates");
                NotifyPropertyChanged("EditorPanelControls");
                NotifyPropertyChanged("TemplatesMenuList");
                NotifyPropertyChanged("HasTemplates");
                ClearDesignerWorkspace();
            }

        }
        
        private bool CanNewTemplate(object parameter) { return ValidWorkspace; }

        private void NewTemplate(object parameter)
        {
            TemplateDesigner window = new TemplateDesigner(_Workspace, this);
            window.ShowDialog();
            NotifyPropertyChanged("ObservableTemplates");
            NotifyPropertyChanged("TemplatesMenuList");
        }

        private bool CanOpenCalculator(object parameter)
        {
            return true;
        }

        private void OpenCalculator(object parameter)
        {
            CalculatorWindow window = new CalculatorWindow();
            window.Show();

        }

        private bool CanNewSet(object parameter) { return ValidWorkspace; }

        private void NewSet(object parameter)
        {
            ICardSetModel set = Workspace.Game.AddSet(Workspace.Game.GetDefaultSetName());
            if (set != null)
            {
                NotifyPropertyChanged("ObservableSets");
            }
        }

        private bool CanNewDeck(object parameter) { return ValidWorkspace; }

        private void NewDeck(object parameter)
        {
            ICardDeckModel deck = Workspace.Game.AddDeck(Workspace.Game.GetDefaultDeckName());
            if (deck != null)
            {
                NotifyPropertyChanged("ObservableDecks");
            }
        }

        protected bool CanNewCard(object parameter) { return ValidWorkspace; }

        protected void NewCard(object parameter)
        {
            string templateName = parameter as string;
            ITemplate template = _Workspace.Game.GetTemplateByName(templateName);
            if (template == null)
            {
                MessageBox.Show("There was an error trying to obtain the template '" + templateName + "'.");
                return;
            }
            ICardSetViewItem cardSet = treeviewSets.SelectedItem as ICardSetViewItem;

            if (cardSet != null)
            {
                ICardViewItem card = new CardViewItem(template);
                cardSet.RegisterCard(card);
                NotifyPropertyChanged("ObservableSets");
                ActiveCard = card; //this will redraw the canvas.
                double x = (canvasWorkspace.ActualWidth / 2) - (ActiveCard.Template.Width / 2);
                double y = (canvasWorkspace.ActualHeight / 2) - (ActiveCard.Template.Height / 2);
                ActiveCard.Template.XPos = x;
                ActiveCard.Template.YPos = y;
                cardSet.IsExpanded = true;
            }
            else
            {
                MessageBox.Show("You must select a set before you can add a card to it.");
            }

        }

        protected bool CanCenterCardView(object param) { return ValidWorkspace; }

        protected void CenterCardView(object parameter)
        {
            if(ActiveCard != null)
            {
                double x = (canvasWorkspace.ActualWidth / 2) - (ActiveCard.Template.Width / 2);
                double y = (canvasWorkspace.ActualHeight / 2) - (ActiveCard.Template.Height / 2);
                ActiveCard.Template.XPos = x;
                ActiveCard.Template.YPos = y;
            }
        }

        protected bool CanReconcileCard(object param) { return ValidWorkspace; }

        protected void ReconcileCard(object parameter)
        {
            if (ActiveCard != null)
            {
                if (Xceed.Wpf.Toolkit.MessageBox.Show(this,
                "The operation you are about to perform is irreversable.\n" +
                "Any elements that have been removed from the master\n" +
                "template will likewise be removed from this card.\n\n" +
                "Click cancel if you do not want this action to be taken.", "WARNING",
                MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                    { return; }

                //find the template that matches this guy
                foreach (ITemplate template in _Workspace.Game.Templates)
                {
                    //BUG ALERT: This should match Guids, not names!!!
                    //UPDATE: In fact it should probably fallback to names if Guid can't be found
                    //But a warning message should be provided to the user before hand.
                    //TODO: Make fallback method here
                    if (template.Guid.Equals(ActiveCard.Template.Guid))
                    {
                        template.ReconcileElementProperties(ActiveCard.Template as BaseElement);
                    }
                }

                NotifyPropertyChanged("ObservableSets");
                RestoreLayoutView(ActiveCard);
            }
        }

        protected bool CanReconcileSet(object param) { return ValidWorkspace; }

        protected void ReconcileSet(object parameter)
        {

            if (Xceed.Wpf.Toolkit.MessageBox.Show(this,
                "The operation you are about to perform is irreversable.\n" +
                "Any elements that have been removed from their master templates\n" +
                "will likewise be removed from the applicable cards within this set.\n\n" +
                "Click cancel if you do not want this action to be taken.", "WARNING",
                MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
            { return; }

            //first, we have to determine the set being clicked
            ICardSetViewItem cardSet = treeviewSets.SelectedItem as ICardSetViewItem;

            if (cardSet != null)
            {
                foreach (ICardModel card in cardSet.Cards)
                {
                    //Yucky inner loop
                    foreach (ITemplate template in _Workspace.Game.Templates)
                    {
                        //BUG ALERT: This should match Guids, not names!!!
                        //UPDATE: In fact it should probably fallback to names if Guid can't be found
                        //But a warning message should be provided to the user before hand.
                        //TODO: Make fallback method here
                        if (template.Guid.Equals(card.Template.Guid))
                        {
                            template.ReconcileElementProperties(card.Template as BaseElement);
                        }
                    }

                    //not the most efficient, but certainly the easiest
                    RestoreLayoutView(card);
                }
                NotifyPropertyChanged("ObservableSets");
            }
            
        }

        protected bool CanLinkCopyCard(object parameter) { return ValidWorkspace; }

        protected void LinkCopyCard(object parameter)
        {
            MessageBox.Show("This feature does not currently work.");
        }

        protected bool CanUniqueCopyCard(object parameter) { return ValidWorkspace; }

        protected void UniqueCopyCard(object parameter)
        {
            if (ActiveCard != null)
            {
                //create new card and move it to a place right after the one that was copied
                ICardViewItem card = ActiveCard.CloneCard(ActiveCard.OwningSet);
                ICardSetModel set = card.Parent as ICardSetModel;
                set.SwitchCardOrder(card, ActiveCard, false);
                
                //change name of the cloned card so that the
                //user can easily identify that it is the cloned card
                foreach(ITemplateElement<string> element in card.Template.Children.OfType<ITemplateElement<string>>())
                {
                    if (element.Name.Equals(Card.ElementNameCardBinding))
                    {
                        element.Content = ActiveCard.Name + " (copy)";
                    }
                }

                NotifyPropertyChanged("ObservableSets");
                double x = (canvasWorkspace.ActualWidth / 2) - (card.Template.Width / 2);
                double y = (canvasWorkspace.ActualHeight / 2) - (card.Template.Height / 2);
                card.Template.XPos = x;
                card.Template.YPos = y;
            }
            NotifyPropertyChanged("ObservableSets");
        }
        #endregion


        #region Drag n Drop
        Point LastDragPos;
        private void treeviewSetsMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //here we are checking for the beginning of dragging events
                Point currentMousePos = e.GetPosition(this.treeviewSets);
                if (Math.Abs(currentMousePos.X - LastDragPos.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentMousePos.Y - LastDragPos.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    ITreeViewItem selectedItem = treeviewSets.SelectedItem as ITreeViewItem;
                    if (selectedItem != null && selectedItem.IsLeaf)
                    {
                        DragDropEffects finalDropEffect = DragDrop.DoDragDrop(this.treeviewSets,
                                                                              new DataObject(typeof(ITreeViewItem).Name, selectedItem),
                                                                              DragDropEffects.Move);
                    }

                }

                LastDragPos = currentMousePos;
            }
        }

        private void treeviewSetsDragEnter(object sender, DragEventArgs e)
        {
            if ( !e.Data.GetDataPresent(typeof(ITreeViewItem).Name) )
            {
                if (e.Source == sender)
                {
                    //we are moving the item to the same control (set-to-set)
                    e.Effects = DragDropEffects.None;
                }
                else
                {
                    //we are moving to another control (set-to-deck)
                    //e.Effects = DragDropEffects.None;
                }
            }
        }

        private void treeviewSetsDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            CardDropAction(sender, e);
        }

        private void CardDropAction(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(typeof(ITreeViewItem).Name))
            {
                TreeViewItem targetItem = sender as TreeViewItem;
                ITreeViewItem target = targetItem.DataContext as ITreeViewItem;
                ITreeViewItem card = e.Data.GetData(typeof(ITreeViewItem).Name) as ITreeViewItem;

                //did we drop onto a card or a deck
                ICardModel dropTarget = target as ICardModel;

                ICardSetViewItem setModel = TreeHelper.InferSet(targetItem);
                if (setModel != null && card != null)
                {
                    ICardSetModel set = setModel as ICardSetModel;
                    if (setModel == card.Parent)
                    {
                        //moving within the same set
                        //did we drop on a deck or a card
                        if (dropTarget != null)
                        {
                            if (dropTarget != card as ICardModel)
                                set.SwitchCardOrder(card as ICardModel, dropTarget, true);
                        }
                    }
                    else
                    {
                        //moving to a new set - first, move to the set
                        ICardSetModel owner = card.Parent as ICardSetModel;
                        owner.UnregisterCard(card as ICardModel);
                        owner = set as ICardSetModel;
                        owner.RegisterCard(card as ICardModel);

                        //did we drop on a deck or a card - move card within the set
                        if (dropTarget != null)
                        {
                            if (dropTarget != card as ICardModel)
                                set.SwitchCardOrder(card as ICardModel, dropTarget, true);
                        }
                    }

                    NotifyPropertyChanged("ObservableSets");
                }
                else
                {
                    MessageBox.Show("There was an error attempting to determine the card or deck during the drag operation.\n " +
                                    "Hell! I don't even know if it was a card or a deck!");
                    return;
                }
                
            }
        }
        #endregion

        
    }
}
