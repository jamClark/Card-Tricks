using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CardTricks.Interfaces
{
    public interface ICardModel : INotifyPropertyChanged
    {
        string Name { get; }
        ITemplate Template { get; }
        Guid Guid { get; }
        ICardSetModel OwningSet { get; }
        IList<ICardSetModel> OwningDecks { get; }
        
        void RegisterWithSet(ICardSetModel set);
        void UnregisterWithSet(ICardSetModel set);
        void RegisterWithDeck(ICardSetModel deck);
        void UnregisterWithDeck(ICardSetModel deck);
        ICardViewItem CloneCard(ICardSetModel owningSet);
    }
}
