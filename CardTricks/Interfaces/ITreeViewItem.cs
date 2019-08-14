using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using CardTricks.Models;

namespace CardTricks.Interfaces
{
    public interface ITreeViewItem : INotifyPropertyChanged
    {
        ObservableCollection<ITreeViewItem> Children { get; }
        string Name { get; }
        bool HasDummyChild { get; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
        bool IsLeaf { get; }
        ITreeViewItem Parent { get; }
        
    }
}
