using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CardTricks.Utils
{
    /// <summary>
    /// Provides several useful methods for obtaining info about a visual tree.
    /// </summary>
    public class WpfTreeHelper
    {
        public static T FindUpVisualTree<T>(DependencyObject initial) where T : DependencyObject
        {
            DependencyObject current = initial;

            while (current != null && current.GetType() != typeof(T))
            {
                current = VisualTreeHelper.GetParent(current);
            }
            return current as T;
        }

        public static Transform GetTransformTree(DependencyObject initial)
        {
            DependencyObject current = initial;
            TransformGroup final = new TransformGroup();

            while (current != null)
            {
                current = VisualTreeHelper.GetParent(current);
                UIElement elm = current as UIElement;
                if (elm != null) final.Children.Add(elm.RenderTransform);
            }

            return final;
        }

        public static Transform GetRotationTree(DependencyObject initial)
        {
            DependencyObject current = initial;
            TransformGroup final = new TransformGroup();

            while (current != null)
            {
                current = VisualTreeHelper.GetParent(current);
                UIElement elm = current as UIElement;
                if (elm != null)
                {
                    RotateTransform rot = elm.RenderTransform as RotateTransform;
                    if(rot != null) final.Children.Add(rot);
                }
            }

            return final;
        }
    }
}
