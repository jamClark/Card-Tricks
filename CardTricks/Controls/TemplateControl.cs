using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CardTricks.Controls
{
    public class TemplateControl : Control
    {
        #region Private Members
        private Canvas canvasContent;
        #endregion


        #region Public Properties
        public UIElement LayoutElement
        {
            get
            {
                //gridContainer.Children.Remove(canvasBackground);
                //return (UIElement)canvasBackground as UIElement;
                return canvasContent as UIElement;
            }
        }

        public double DPI
        {
            get { return 96.0; }
        }

        public Size LayoutDimension
        {
            get
            {
                return new Size(this.ActualWidth, this.ActualHeight);
            }
        }

        #endregion


        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        public TemplateControl()
        {
            canvasContent = new Canvas();
            canvasContent.Height = Double.NaN;
            canvasContent.Width = Double.NaN;
            canvasContent.VerticalAlignment = VerticalAlignment.Stretch;
            canvasContent.HorizontalAlignment = HorizontalAlignment.Stretch;
            canvasContent.Background = Brushes.Gray;

            this.AddVisualChild(canvasContent);
            
            this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            this.Arrange(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));
            this.UpdateLayout();
        }

        #endregion
    }
}
