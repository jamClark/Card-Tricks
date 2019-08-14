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

namespace CardTricks.Controls
{
    /// <summary>
    /// Interaction logic for SuperRichTextBox.xaml
    /// </summary>
    public partial class SuperRichTextBox : UserControl
    {
        public FlowDocument Document
        {
            get { return (FlowDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(FlowDocument), typeof(SuperRichTextBox), new PropertyMetadata(OnDocumentChanged));

        private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SuperRichTextBox control = (SuperRichTextBox)d;
            if (e.NewValue == null)
                control.rtbText.Document = new FlowDocument(); //Document is not amused by null :)

            control.rtbText.Document = e.NewValue as FlowDocument;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SuperRichTextBox()
        {
            InitializeComponent();
            
        }
        
    }
}
