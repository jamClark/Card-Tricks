using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CardTricks.Interfaces;
using System.Runtime.Serialization;

namespace CardTricks.Models
{
    /// <summary>
    /// Similar to a card set but this allows any number of copies
    /// of the same card to exist within it.
    /// </summary>
    [KnownType(typeof(Card))]
    [KnownType(typeof(CardViewItem))]
    [KnownType(typeof(DeckViewItem))]
    [KnownType(typeof(CardSet))]
    [KnownType(typeof(Deck))]
    [DataContract]
    public class Deck : CardSet
    {
        #region Private Members
        [DataMember]
        private List<int> Multiples;

        #endregion

        
        #region Public Methods
        public Deck(string name) : base(name)
        {
            Multiples = new List<int>();
        }

        /// <summary>
        /// Never use this constructor! It is for Data Contract serialization only!
        /// </summary>
        public Deck() : this("")
        {
        }

        override public void RegisterCard(ICardModel card)
        {
            if (card == null) return;
            ValidateCards();
            if (Multiples == null) Multiples = new List<int>();
            int index = _Cards.IndexOf(card as Card);
            if (index < 0)
            {
                //registering a new card for the first time
                card.RegisterWithDeck(this);
                _Cards.Add(card as Card);
                Multiples.Add(1);
            }
            else
            {
                Multiples[index]++;
            }
            
        }

        override public void UnregisterCard(ICardModel card)
        {
            ValidateCards();
            if (Multiples == null) Multiples = new List<int>();
            int index = _Cards.IndexOf(card as Card);
            if (index > 0)
            {
                //we have multiples of this card, remove one
                Multiples[index]--;
            }
            else if (index == 1)
            {
                //this is the last multiple, complete remove this card
                card.UnregisterWithDeck(this);
                Multiples.RemoveAt(index);
                _Cards.Remove(card as Card);
            }
            
        }

        override public int GetMultiples(ICardModel card)
        {
            ValidateCards();
            int index = _Cards.IndexOf(card as Card);
            if (index < 0) return 0;
            return Multiples[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moving"></param>
        /// <param name="location"></param>
        /// <param name="before"></param>
        override public void SwitchCardOrder(ICardModel moving, ICardModel location, bool before = true)
        {
            ValidateCards();
            if (Multiples == null) Multiples = new List<int>();
            if (moving == null || location == null) return;
            if (Multiples.Count != _Cards.Count) throw new Exception("Invalid state. The number of cards does not match the multiples indexer.");
            //TODO: We need to move the index around for the Multiples!!
            int indexMoving = _Cards.IndexOf(moving as Card);
            if(indexMoving < 0) throw new Exception("Invalid state. The moving card's multiples indexer is out of sync.");
            int multiples = Multiples[indexMoving];
            
            _Cards.Remove(moving as Card);
            int indexLocation = _Cards.IndexOf(location as Card);
            if(indexLocation < 0) throw new Exception("Invalid state. The location card's multiples indexer is out of sync.");

            
            for (int index = 0; index < _Cards.Count; index++)
            {
                if (_Cards[index] == location)
                {
                    if (before)
                    {
                        //place card and multiples just before new location reference point
                        _Cards.Insert(index, moving as Card);
                        Multiples.Insert(index, multiples);
                    }
                    else
                    {
                        //place card and multiples just after new location reference point
                        _Cards.Insert(index + 1, moving as Card);
                        Multiples.Insert(index + 1, multiples);
                    }
                    return;
                }
            }

            return;
        }
        #endregion
    }
}
