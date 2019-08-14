using CardTricks.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CardTricks.Models
{
    /// <summary>
    /// Provides and interface for CardSets to interact with treeviews.
    /// </summary>
    [KnownType(typeof(Card))]
    [KnownType(typeof(CardViewItem))]
    [KnownType(typeof(DeckViewItem))]
    [KnownType(typeof(CardSet))]
    [KnownType(typeof(Deck))]
    [Serializable]
    public class DeckViewItem : Deck, ICardDeckViewItem, ITreeViewItem
    {
        public DeckViewItem(string setName) : base(setName)
        {
        }

        /// <summary>
        /// Never use this constructor! It is for Data Contract serialization only!
        /// </summary>
        public DeckViewItem()
            : this("")
        {
        }

        public ObservableCollection<ITreeViewItem> Children
        {
            get
            {
                //HACK ALERT:
                List<ITreeViewItem> list = new List<ITreeViewItem>();
                foreach (ICardModel card in _Cards)
                {
                    list.Add((ITreeViewItem)card);
                }
                return new ObservableCollection<ITreeViewItem>(list);
            }
        }

        public bool HasDummyChild
        {
            get { return true; }
        }

        private bool _IsSelected = false;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (value != _IsSelected)
                {
                    _IsSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        private bool _IsExpanded;
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                if (value != _IsExpanded)
                {
                    _IsExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_IsExpanded && Parent != null) Parent.IsExpanded = true;
            }
        }

        public bool IsLeaf
        {
            get { return false; }
        }

        public ITreeViewItem Parent
        {
            get { return null; }
        }
    }
}
