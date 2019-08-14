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
    public partial class ProjectNameWindow : Window
    {
        public string ProjectName = "";
        public string GameName = "";

        public ProjectNameWindow(string title, string projectName, string gameName)
        {
            InitializeComponent();

            this.Title = title;
            this.textboxProjectName.Text = projectName;
            this.textboxGameName.Text = gameName;

            ProjectName = projectName;
            GameName = gameName;
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            textboxProjectName.Focus();
            textboxProjectName.SelectAll();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProjectName = textboxProjectName.Text;
            GameName = textboxGameName.Text;
            this.DialogResult = true;
        }
    }
}
