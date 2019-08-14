using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardTricks.Interfaces
{
    public interface ICardDeckViewItem : ICardDeckModel, ITreeViewItem, INotifyPropertyChanged
    {
        
    }
}
