using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CardTricks.Models;
using CardTricks.Interfaces;
using System.Windows.Media;

namespace CardTricks.Utils
{
    public static class TreeHelper
    {
        /// <summary>
        /// Returns the item as CardSetModelView if it represents a deck. If it represents
        /// a card it returns that card's visual parent which should be a deck.
        /// </summary>
        /// <param name="item"></param>
        public static ICardSetViewItem InferSet(TreeViewItem item, DragEventArgs e = null)
        {
            ITreeViewItem itemData = item.DataContext as ITreeViewItem;
            if (itemData != null && itemData.IsLeaf)
            {
                //this is a child, get its parent
                TreeViewItem parent = GetParentTreeViewItem(item as DependencyObject);

                if (parent == null) return null;
                ICardSetViewItem set = parent.DataContext as ICardSetViewItem;
                if (set != null && !set.IsLeaf) return set as ICardSetViewItem;
            }
            else
            {
                //this is a parent, return it
                return item.DataContext as ICardSetViewItem;
            }

            return null;
        }

        /// <summary>
        /// Returns the item as CardSetModelView if it represents a deck. If it represents
        /// a card it returns that card's visual parent which should be a deck.
        /// </summary>
        /// <param name="item"></param>
        public static ICardDeckViewItem InferDeck(TreeViewItem item, DragEventArgs e = null)
        {
            ITreeViewItem itemData = item.DataContext as ITreeViewItem;
            if (itemData != null && itemData.IsLeaf)
            {
                //this is a child, get its parent
                TreeViewItem parent = GetParentTreeViewItem(item as DependencyObject);

                if (parent == null) return null;
                ICardDeckViewItem set = parent.DataContext as ICardDeckViewItem;
                if (set != null && !set.IsLeaf) return set as ICardDeckViewItem;
            }
            else
            {
                //this is a parent, return it
                return item.DataContext as ICardDeckViewItem;
            }

            return null;
        }

        /// <summary>
        /// Returns the visual parent of the given treeview item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static TreeViewItem GetParentTreeViewItem(DependencyObject item)
        {
            if (item != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(item);
                TreeViewItem parentTreeViewItem = parent as TreeViewItem;
                return parentTreeViewItem ?? GetParentTreeViewItem(parent);
            }

            return null;
        }


    }
}
