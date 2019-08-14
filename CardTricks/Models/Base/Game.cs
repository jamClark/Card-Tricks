using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CardTricks.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using CardTricks.Attributes;
using CardTricks.Models.Elements;

namespace CardTricks.Models
{
    /// <summary>
    /// Contains all templates, cards, sets,
    /// and decks that describe a game.
    /// </summary>
    [KnownType(typeof(TextRegionElement))]
    [KnownType(typeof(Template))]
    [SaveableObject]
    [DataContract]
    public class Game : ViewModel, INotifyPropertyChanged
    {
        #region Private Members
        [Saveable(Name = "Templates", Concrete = typeof(List<Template>))]
        [DataMember(Name = "Templates", Order = 4)]
        private List<Template> _Templates;
        [Saveable(Name="Sets", Concrete = typeof(List<SetViewItem>))]
        [DataMember(Name="Sets", Order = 5)]
        private List<SetViewItem> _Sets;
        [Saveable(Name = "Decks", Concrete = typeof(List<DeckViewItem>))]
        [DataMember(Name = "Decks", Order = 6)]
        private List<DeckViewItem> _Decks;
        

        //these are used for generating unique deck, set, and template names
        [Saveable]
        [DataMember(Order = 1)]
        private int SetsMade = 0;
        [Saveable]
        [DataMember(Order = 2)]
        private int DecksMade = 0;
        [Saveable]
        [DataMember(Order = 3)]
        private int TemplatesMade = 0;
        #endregion


        #region Public Properties
        [Saveable]
        [DataMember(Order=0)]
        public string Name { get; private set; }

        public IList<ICardSetViewItem> Sets
        {
            get
            { 
                //BUG ALERT: THIS WON'T WORK!!!! WE NEED TO CONVERT MANUALLY
                //return _Sets as IList<ICardSetViewItem>;

                IList<ICardSetViewItem> list = new List<ICardSetViewItem>();
                foreach (SetViewItem s in _Sets) list.Add(s);
                return list;
            }
            private set
            {
                if (_Sets != value)
                {
                    _Sets = value as List<SetViewItem>;
                    NotifyPropertyChanged("Sets");
                    NotifyPropertyChanged("ObservableSets");
                }
            }
        }

        public IList<ITemplate> Templates
        {
            get 
            {
                //BUG ALERT: THIS WON'T WORK!!!! WE NEED TO CONVERT MANUALLY
                //return _Templates as IList<ITemplate>;
                
                //ALERT: THIS ALSO WON'T WORK SINCE WE NEED A REF TO THE ORIGINAL LIST
                IList<ITemplate> list = new List<ITemplate>();
                foreach (Template t in _Templates) list.Add(t);
                return list;
                //IList<Template> list = _Templates as IList<Template>;
                //return 
            }
            private set
            {
                if (_Templates != value)
                {
                    _Templates = value as List<Template>;
                    NotifyPropertyChanged("Templates");
                    NotifyPropertyChanged("ObservableTemplates");
                    NotifyPropertyChanged("HasTemplates");
                }
            }
        }

        public ObservableCollection<ICardSetViewItem> ObservableSets
        {
            get
            {
                return new ObservableCollection<ICardSetViewItem>(_Sets);
            }
        }

        public ObservableCollection<ICardDeckViewItem> ObservableDecks
        {
            get
            {
                return new ObservableCollection<ICardDeckViewItem>(_Decks);
            }
        }
        
        public ObservableCollection<ITemplate> ObservableTemplates
        {
            get
            {
                return new ObservableCollection<ITemplate>(Templates);
            }
        }

        public bool HasTemplates
        {
            get
            {
                if (ObservableTemplates == null) return false;
                return (ObservableTemplates.Count > 0);
            }
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        public Game(string name)
        {
            Initialize();
            Name = name;
        }

        /// <summary>
        /// Default constructor. Used for DataContract deserialization.
        /// </summary>
        private Game() : this(null)
        {
        }

        private void Initialize()
        {
            Name = "DEFAULT NAME";
            ValidateSets();
            ValidateDecks();
            ValidateTemplates();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Initialize();
        }
        
        public ICardSetModel AddSet(string setName)
        {
            ValidateSets();
            if (setName == null || setName.Length < 1) return null;
            foreach (ICardSetModel set in _Sets)
            {
                if (set.Name.Equals(setName)) throw new InvalidOperationException("A set with the name '" + setName + "' already exists.");
            }

            ICardSetViewItem s = new SetViewItem(setName);
            _Sets.Add(s as SetViewItem);
            SetsMade++;
            return s;
        }

        public void RemoveSet(ICardSetViewItem set)
        {
            ValidateSets();
            if (set == null) return;

            //TODO: All cards in this set must be removed from _Decks as well
            _Sets.Remove(set as SetViewItem);
        }

        public void SwitchSetOrder(ICardSetViewItem moving, ICardSetViewItem location, bool before = true)
        {
            ValidateSets();
            if (moving == null || location == null) return;

            _Sets.Remove(moving as SetViewItem);
            for (int index = 0; index < _Sets.Count; index++)
            {
                if (_Sets[index] == location)
                {
                    if (before) _Sets.Insert(index, moving as SetViewItem);
                    else _Sets.Insert(index + 1, moving as SetViewItem);
                    return;
                }
            }

            return;
        }

        public ICardSetModel GetSetByName(string name)
        {
            ValidateSets();
            if (name == null || name.Length < 1) return null;
            foreach (ICardSetModel set in _Sets)
            {
                if (set.Name.Equals(name)) return set;
            }
            return null;
        }

        public bool RenameSet(ICardSetModel set, string newName)
        {
            ValidateSets();
            if (set == null) return false;
            if(!_Sets.Contains(set)) throw new InvalidOperationException("The set '" + set.Name + "' does not exist within this game project.");

            //first, make sure the new name isn't invalid
            foreach(ICardSetModel s in _Sets)
            {
                if(s.Name.Equals(newName)) throw new InvalidOperationException("A set with the name '" + newName + "' already exists.");
            }

            return true;
        }

        public string GetDefaultSetName()
        {
            return "New Set " + SetsMade;
        }



        public ICardDeckViewItem AddDeck(string deckName)
        {
            ValidateDecks();
            if (deckName == null || deckName.Length < 1) return null;
            foreach (ICardDeckModel deck in _Decks)
            {
                if (deck.Name.Equals(deckName)) throw new InvalidOperationException("A deck with the name '" + deckName + "' already exists.");
            }

            ICardDeckViewItem d = new DeckViewItem(deckName);
            _Decks.Add(d as DeckViewItem);
            DecksMade++;
            return d;
        }

        public void RemoveDeck(ICardDeckViewItem deck)
        {
            ValidateDecks();
            if (deck == null) return;
            _Decks.Remove(deck as DeckViewItem);
        }

        public void SwitchDeckOrder(ICardDeckViewItem moving, ICardDeckViewItem location, bool before = true)
        {
            ValidateDecks();
            if (moving == null || location == null) return;

            _Decks.Remove(moving as DeckViewItem);
            for (int index = 0; index < _Decks.Count; index++)
            {
                if (_Decks[index] == location)
                {
                    if (before) _Decks.Insert(index, moving as DeckViewItem);
                    else _Decks.Insert(index + 1, moving as DeckViewItem);
                    return;
                }
            }

            return;
        }

        public ICardDeckViewItem GetDeckByName(string name)
        {
            ValidateDecks();
            if (name == null || name.Length < 1) return null;
            foreach (ICardDeckViewItem deck in _Decks)
            {
                if ( ((ICardDeckModel)deck).Name.Equals(name)) return deck;
            }
            return null;
        }

        public bool RenameDeck(ICardDeckViewItem deck, string newName)
        {
            ValidateDecks();
            if (deck == null) return false;
            if (!_Decks.Contains(deck)) 
                throw new InvalidOperationException("The deck '" + ((ICardDeckModel)deck).Name + "' does not exist within this game project.");

            //first, make sure the new name isn't invalid
            foreach (ICardDeckViewItem s in _Decks)
            {
                if ( ((ICardDeckModel)s).Name.Equals(newName)) 
                    throw new InvalidOperationException("A deck with the name '" + newName + "' already exists.");
            }

            return true;
        }

        public string GetDefaultDeckName()
        {
            return "New Deck " + DecksMade;
        }



        public ITemplate AddTemplate(string name, ITemplate parent = null)
        {
            ValidateTemplates();
            if (name == null || name.Length < 1) return null;
            foreach (ITemplate temp in _Templates)
            {
                if (temp.Name.Equals(name)) throw new InvalidOperationException("A template with the name '" + name + "' already exists.");
            }

            ITemplate t = new Template(name, parent);
            TemplatesMade++;
            _Templates.Add(t as Template);
            
            NotifyPropertyChanged("Templates");
            NotifyPropertyChanged("ObservableTemplates");
            return t;
        }

        public void RemoveTemplate(ITemplate template)
        {
            ValidateTemplates();
            if (template == null) return;
            
            //TODO: all cards using this template must be removed from
            //all sets and decks.
            _Templates.Remove(template as Template);
        }

        public ITemplate GetTemplateByName(string name)
        {
            ValidateTemplates();
            if (name == null || name.Length < 1) return null;
            foreach (ITemplate temp in _Templates)
            {
                if (temp.Name.Equals(name)) return temp;
            }
            return null;
        }

        public bool RenameTemplate(ITemplate template, string newName)
        {
            ValidateTemplates();
            if (template == null) return false;
            if (!_Templates.Contains(template)) throw new InvalidOperationException("The template '" + template.Name + "' does not exist within this game project.");

            //first, make sure the new name isn't invalid
            foreach (ITemplate t in _Templates)
            {
                if (t.Name.Equals(newName)) throw new InvalidOperationException("A template with the name '" + newName + "' already exists.");
            }

            return true;
        }

        public string GetDefaultTemplateName()
        {
            return "Template " + TemplatesMade;
        }
        #endregion


        #region Private Methods
        private void ValidateSets()
        {
            if (_Sets == null) _Sets = new List<SetViewItem>();
        }

        private void ValidateDecks()
        {
            if (_Decks == null) _Decks = new List<DeckViewItem>();
        }

        private void ValidateTemplates()
        {
            if (_Templates == null) _Templates = new List<Template>();
        }
        #endregion
    }
}
