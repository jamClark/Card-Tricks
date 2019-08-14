using CardTricks.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CardTricks.Models
{
    /// <summary>
    /// Provides an interface for Cards to interact with treeviews.
    /// </summary>
    [KnownType(typeof(Card))]
    [KnownType(typeof(CardViewItem))]
    [KnownType(typeof(DeckViewItem))]
    [KnownType(typeof(CardSet))]
    [KnownType(typeof(Deck))]
    [DataContract]
    public class CardViewItem : Card, ICardViewItem
    {
        public CardViewItem(ITemplate template)
            : base(template)
        {
        }

        /// <summary>
        /// Never use this constructor! It is for Data Contract serialization only!
        /// </summary>
        public CardViewItem()
            : this(null)
        {
        }

        public ObservableCollection<ITreeViewItem> Children { get { return null; } }

        public bool HasDummyChild
        {
            get { return false; }
        }

        //private bool _IsExpanded;
        public bool IsExpanded
        {
            get { return false;}// _IsExpanded; }
            set
            {
                //if (value != _IsExpanded)
                //{
                //    _IsExpanded = value;
                //    NotifyPropertyChanged("IsExpanded");
                //}

                //// Expand all the way up to the root.
                //if (_IsExpanded && Parent != null) Parent.IsExpanded = true;
            }
        }

        [DataMember(Name = "IsSelected")]
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

        public bool IsLeaf
        {
            get { return true; }
        }

        public ITreeViewItem Parent
        {
            get { return (ITreeViewItem)_Set; }
        }
    }
}
