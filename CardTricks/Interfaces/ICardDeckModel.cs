using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardTricks.Interfaces
{
    public interface ICardDeckModel
    {
        string Name { get; }
        IList<ICardModel> Cards { get; }
        ObservableCollection<ICardModel> ObservableCards { get; }

        void RegisterCard(ICardModel card);
        void UnregisterCard(ICardModel card);
        int GetMultiples(ICardModel card);

        void SwitchCardOrder(ICardModel moving, ICardModel location, bool before = true);
    }
}
