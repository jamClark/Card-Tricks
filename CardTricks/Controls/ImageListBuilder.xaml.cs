using CardTricks.Utils;
using CardTricks.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace CardTricks.Controls
{
    /// <summary>
    /// Interaction logic for ImageListBuilder.xaml
    /// </summary>
    public partial class ImageListBuilder : UserControl
    {
        private int ActiveSelection = -1;


        #region Public Properties
        public IList<string> ImageFiles
        {
            get { return (IList<string>)GetValue(ImageFilesProperty); }
            set { SetValue(ImageFilesProperty, value); }
        }

        public string SelectedFile
        {
            get { return (string)GetValue(SelectedFileProperty); }
            set { SetValue(SelectedFileProperty, value); }
        }
        #endregion


        #region Dependency Properties
        public static readonly DependencyProperty ImageFilesProperty =
            DependencyProperty.Register("ImageFiles", typeof(IList<string>), typeof(ImageListBuilder), new PropertyMetadata(OnListChanged));

        private static void OnListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageListBuilder control = (ImageListBuilder)d;
            if (e.NewValue == null) control.ImageFiles = new List<string>(); //list is not amused by null :)
            if(control.ImageFiles == null) control.ImageFiles = new List<string>();

            control.ImageFiles = e.NewValue as List<string>;
        }

        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register("SelectedFile", typeof(string), typeof(ImageListBuilder), new PropertyMetadata(OnSelectionChanged));

        private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageListBuilder control = (ImageListBuilder)d;
            control.SelectedFile = e.NewValue as string;
        }
        #endregion


        /// <summary>
        /// Constructor.
        /// </summary>
        public ImageListBuilder()
        {
            InitializeComponent();
            SetValue(ImageFilesProperty, new List<string>());
        }

        private void OnClickButton_AddImage(object sender, RoutedEventArgs e)
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
                
                listboxImages.Items.Add(file);
                ImageFiles.Add(file);
            }
        }

        private void OnButtonClick_RemoveSelection(object sender, RoutedEventArgs e)
        {
            if(ActiveSelection < 0) return;
            ListBox box = this.listboxImages;// sender as ListBox;
            if (box != null && box.Items.Count > 0)
            {
                ImageFiles.RemoveAt(ActiveSelection);
                box.Items.RemoveAt(ActiveSelection);
                SelectedFile = null;
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox box = sender as ListBox;
            if (box != null && box.Items.Count > 0)
            {
                ActiveSelection = box.SelectedIndex;
                SelectedFile = box.SelectedItem as string;
            }
        }
    }
}
