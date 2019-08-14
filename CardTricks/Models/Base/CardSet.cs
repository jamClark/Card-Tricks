using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using CardTricks.Interfaces;
using CardTricks.Models;
using CardTricks.Cmds;
using System.Windows.Input;
using System.Runtime.Serialization;

namespace CardTricks.Models
{
    /// <summary>
    /// A collection of cards. Each card is unique within this set.
    /// </summary>
    [KnownType(typeof(Card))]
    [KnownType(typeof(CardViewItem))]
    [KnownType(typeof(DeckViewItem))]
    [KnownType(typeof(CardSet))]
    [KnownType(typeof(Deck))]
    [DataContract]
    public class CardSet : ViewModel, ICardSetModel
    {
        #region Private Members
        [DataMember(Name="Name", Order=0)]
        protected string _Name = "";
        [DataMember(Name = "Guid", Order = 1)]
        protected Guid _Guid = System.Guid.NewGuid();
        [DataMember(Name = "Cards", Order = 2)]
        protected List<Card> _Cards;
        #endregion


        #region Public Properties
        public string Name
        {
            get { return _Name;}
            set
            {
                if(_Name == null || !_Name.Equals(value))
                {
                    _Name = value;
                    //NotifyPropertyChanged("Name");
                }
            }
        }

        public Guid Guid { get { return _Guid; } }

        public IList<ICardModel> Cards
        {
            get
            {
                IList<ICardModel> list = new List<ICardModel>();
                foreach (Card c in _Cards) list.Add(c);
                return list;
            }
            set
            {
                if (_Cards == null || !_Cards.Equals(value))
                {
                    _Cards = value as List<Card>;
                    NotifyPropertyChanged("Cards");
                    NotifyPropertyChanged("ObservableCards");
                }
            }
        }

        public ObservableCollection<ICardModel> ObservableCards
        {
            get { return new ObservableCollection<ICardModel>(_Cards); }
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        public CardSet(string setName)
        {
            ValidateCards();
            _Name = setName;
        }

        /// <summary>
        /// Never use this constructor! It is for Data Contract serialization only!
        /// </summary>
        public CardSet() : this("")
        {

        }

        virtual public void RegisterCard(ICardModel card)
        {
            ValidateCards();
            if (!_Cards.Contains(card))
            {
                card.RegisterWithSet(this);
                _Cards.Add(card as Card);
                NotifyPropertyChanged("Cards");
                NotifyPropertyChanged("ObservableCards");
            }
        }

        virtual public void UnregisterCard(ICardModel card)
        {
            ValidateCards();
            if (_Cards.Contains(card))
            {
                _Cards.Remove(card as Card);
                card.UnregisterWithSet(this); // this will remove this card from all decks.

                NotifyPropertyChanged("Cards");
                NotifyPropertyChanged("ObservableCards");
                //this card should be deleted at this point so it doesn't
                //need to unregister with any sets
            }
        }

        virtual public int GetMultiples(ICardModel card)
        {
            ValidateCards();
            if(_Cards.Contains(card)) return 1;
            return 0;
        }

        /// <summary>
        /// Moves a card to a position that is either before or after another card within the set.
        /// </summary>
        /// <param name="moving">The card that is moving.</param>
        /// <param name="location">The card that marks the position where the moving card will go.</param>
        /// <param name="before">True if moving to a position before the location. False if after.</param>
        virtual public void SwitchCardOrder(ICardModel moving, ICardModel location, bool before = true)
        {
            ValidateCards();
            if (moving == null || location == null) return;

            _Cards.Remove(moving as Card);
            for (int index = 0; index < _Cards.Count; index++)
            {
                if (_Cards[index] == location)
                {
                    if (before) _Cards.Insert(index, moving as Card);
                    else _Cards.Insert(index + 1, moving as Card);
                    return;
                }
            }

            return;
        }
        #endregion


        #region Private Methods
        protected void ValidateCards()
        {
            if (_Cards == null) _Cards = new List<Card>();
        }
        
        #endregion


        
    }
}
