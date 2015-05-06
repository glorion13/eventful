using Eventful.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Eventful.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Initialise();
            if (IsInDesignMode)
            {
                InitialiseInDesignMode();
            }
            else
            {
                InitialiseInDesignMode();
                //InitialiseInRealMode();
            }
        }

        private void Initialise()
        {
            Decks = new ObservableCollection<Deck>();
            DeckFilter = "";
            StatusbarVisibility = true;
            DecksViewSource = new CollectionViewSource();
            DecksViewSource.Source = Decks;
            EventsViewSource = new CollectionViewSource();
        }

        private void InitialiseInDesignMode()
        {
            Deck mainDeck = new Deck("Main");
            Deck undeadDeck = new Deck("Undead Army");
            mainDeck.Events.Add(new Event("Finding Timmy"));
            mainDeck.Events.Add(new Event("Cinding Timmy"));
            mainDeck.Events.Add(new Event("Ainding Timmy"));
            mainDeck.Events.Add(new Event("Binding Timmy"));
            Decks.Add(mainDeck);
            Decks.Add(undeadDeck);
            SelectedDeck = mainDeck;
        }

        private void InitialiseInRealMode()
        {
        }

        private bool TitleContains(object obj)
        {
            Deck deck = obj as Deck;
            if (deck == null) return false;
            return deck.Title.Contains(DeckFilter);
        }

        private ObservableCollection<Deck> decks;
        public ObservableCollection<Deck> Decks
        {
            get
            {
                return decks;
            }
            set
            {
                Set(() => Decks, ref decks, value);
            }
        }

        public CollectionViewSource DecksViewSource { get; set; }
        public CollectionViewSource EventsViewSource { get; set; }

        private Deck selectedDeck;
        public Deck SelectedDeck
        {
            get
            {
                return selectedDeck;
            }
            set
            {
                Set(() => SelectedDeck, ref selectedDeck, value);
            }
        }

        private Event selectedEvent;
        public Event SelectedEvent
        {
            get
            {
                return selectedEvent;
            }
            set
            {
                Set(() => SelectedEvent, ref selectedEvent, value);
            }
        }

        private bool statusbarVisibility = true;
        public bool StatusbarVisibility
        {
            get
            {
                return statusbarVisibility;
            }
            set
            {
                Set(() => StatusbarVisibility, ref statusbarVisibility, value);
            }
        }

        private string deckFilter;
        public string DeckFilter
        {
            get
            {
                return deckFilter;
            }
            set
            {
                Set(() => DeckFilter, ref deckFilter, value);
                if (DecksViewSource != null)
                    DecksViewSource.View.Filter = new Predicate<object>(TitleContains);
            }
        }

        private string eventFilter;
        public string EventFilter
        {
            get
            {
                return eventFilter;
            }
            set
            {
                Set(() => EventFilter, ref eventFilter, value);
                if (EventsViewSource != null)
                    EventsViewSource.View.Filter = new Predicate<object>(TitleContains);
            }
        }

    }
}