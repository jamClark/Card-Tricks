using CardTricks.Interfaces;
using CardTricks.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace CardTricks.Converters
{
    public class MultiplesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //BUG: card names will not update in realtime like this
            TreeViewItem cardItem = values[1] as TreeViewItem;
            TreeViewItem deckItem = values[2] as TreeViewItem;
            
            ////TreeView view = parameter as TreeView;
            ITreeViewItem cardModel = cardItem.DataContext as ITreeViewItem;
            ICardDeckViewItem deck = null;
            if (values.Length > 1 && deckItem != null && deckItem.DataContext != null)
            {
                deck = TreeHelper.InferDeck(cardItem);// deckItem.DataContext as ICardDeckViewItem;
            }

            ////return deck names
            if (cardModel.IsLeaf)
            {
                //MessageBox.Show(deckItem.Name);
                if (deck != null)
                {
                    int mul = deck.GetMultiples(cardModel as ICardModel);
                    if (mul != 1) return cardModel.Name + "  x" + mul;
                }
                return cardModel.Name;
                
            }

            ////we need to see how many of these guys are in the deck
            return cardModel.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private TreeViewItem GetNearestContainer(UIElement element)
        {

            // Walk up the element tree to the nearest tree view item.

            TreeViewItem container = element as TreeViewItem;

            while ((container == null) && (element != null))
            {

                element = VisualTreeHelper.GetParent(element) as UIElement;

                container = element as TreeViewItem;

            }

            return container;

        }
        
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
