using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CardTricks.Interfaces;
using CardTricks.Models;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using CardTricks.Utils;
using CardTricks.Cmds;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace CardTricks.Models
{
    /// <summary>
    /// Represents a single card. A card is an instance of
    /// a 'template chain' that belongs to a 'set' and any
    /// number of 'decks'.
    /// </summary>
    [KnownType(typeof(Card))]
    [KnownType(typeof(CardViewItem))]
    [KnownType(typeof(DeckViewItem))]
    [KnownType(typeof(CardSet))]
    [KnownType(typeof(Deck))]
    [KnownType(typeof(Template))]
    [KnownType(typeof(BaseElement))]
    [DataContract]
    public class Card : ViewModel, ICardModel, INotifyPropertyChanged
    {
        public static readonly string ElementNameCardBinding = "$Name";
        
        
        #region Private Members
        [DataMember(Name = "Guid", Order = 0)]
        private Guid _Guid;
        [DataMember(Name = "Templates", Order = 1)]
        protected ITemplate _Template;
        [DataMember(Name = "Set", Order = 2)]
        protected ICardSetModel _Set; //can only belong to one set.
        [DataMember(Name = "Decks", Order = 3)]
        protected IList<ICardSetModel> _Decks; //can be contained within many decks.

        [NonSerialized]
        private IElement NameElement = null;
        #endregion


        #region Public Properties
        public string Name
        {
            get
            {
                if (_Template == null) return "<Card>";
                if (NameElement != null && NameElement.Name.Equals(ElementNameCardBinding))
                {
                    //still using the old element ref
                    //return NameElement.Content;
                    return NameElement.ContentToString;
                }
                
                if (NameElement != null) NameElement.NameChanged -= NameChangedHandler;

                
                foreach (IElement element in _Template.Children)
                {
                    if (element.Name.Equals(ElementNameCardBinding))
                    {
                        NameElement = element;
                        NameElement.NameChanged += NameChangedHandler;
                        return NameElement.ContentToString;
                    }
                }
                return "<Card>";
            }
        }

        /// <summary>
        /// This handles the name change event from the element that is tied to it.
        /// This allows the element's updating control to update the set and deck lists in real-time.
        /// </summary>
        protected void NameChangedHandler()
        {
            NotifyPropertyChanged("Name"); 
        }

        public Guid Guid
        {
            get { return _Guid; }
        }

        public ICardSetModel OwningSet
        {
            get { return _Set; }
        }

        public IList<ICardSetModel> OwningDecks
        {
            get { return _Decks; }
        }

        public ITemplate Template
        {
            get { return _Template; }
        }
        #endregion
        

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        public Card(ITemplate template)
        {
            _Template = template.GetShallowClone() as ITemplate;
            _Guid =System.Guid.NewGuid();
            _Decks = new List<ICardSetModel>();
        }

        /// <summary>
        /// Never use this constructor! It is for Data Contract serialization only!
        /// </summary>
        public Card()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        public void RegisterWithSet(ICardSetModel set)
        {
            _Set = set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        public void UnregisterWithSet(ICardSetModel set)
        {
            if (set == _Set)
            {
                //remove this card from all decks
                if (_Decks.Count > 1)
                {
                    for (int i = _Decks.Count - 1; i >= 0; i--)
                    {
                        _Decks[i].UnregisterCard(this);
                    }
                }
                _Set = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deck"></param>
        public void RegisterWithDeck(ICardSetModel deck)
        {
            if (deck == null) return;
            if (_Decks == null) _Decks = new List<ICardSetModel>();
            if (!_Decks.Contains(deck)) _Decks.Add(deck);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deck"></param>
        public void UnregisterWithDeck(ICardSetModel deck)
        {
            if (_Decks == null) _Decks = new List<ICardSetModel>();
            if (!_Decks.Contains(deck)) _Decks.Remove(deck);
        }

        /// <summary>
        /// Creates an exact copy of this card but for two differences.
        /// It has a newly generated Guid and can be assigned to a new owning set.
        /// This clone is effectively a whole new card with the same template
        /// and element values as the source it was cloned from. There are no links
        /// shared between this card the new clone.
        /// </summary>
        /// <param name="owningSet"></param>
        /// <returns></returns>
        public ICardViewItem CloneCard(ICardSetModel owningSet)
        {
            ICardViewItem card = new CardViewItem(this.Template);
            owningSet.RegisterCard(card);
            return card;
        }

        #endregion

    }
}
