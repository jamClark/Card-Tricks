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

namespace CardTricks.Windows
{
    /// <summary>
    /// Interaction logic for RenamerWindow.xaml
    /// </summary>
    public partial class RenamerWindow : Window
    {
        public string NewName = "";

        public RenamerWindow(string title, string defaultName)
        {
            InitializeComponent();

            this.Title = title;
            this.textboxName.Text = defaultName;
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            textboxName.Focus();
            textboxName.SelectAll();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            NewName = textboxName.Text;
            this.DialogResult = true;
        }
    }
}
