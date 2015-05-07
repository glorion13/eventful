using Eventful.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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
                //InitialiseInDesignMode();
                InitialiseInRealMode();
            }
        }

        private void Initialise()
        {
            Decks = new ObservableCollection<Deck>();
            DeckFilter = "";
            DecksViewSource = new CollectionViewSource();
            DecksViewSource.Source = Decks;
            EventsViewSource = new CollectionViewSource();
        }
        private void InitialiseInDesignMode()
        {
            Deck mainDeck = new Deck("Main");
            Deck undeadDeck = new Deck("Undead Army");
            mainDeck.Events.Add(new Event("Finding Timmy: A Bewildering Adventure"));
            mainDeck.Events.Add(new Event("Cinding Timmy"));
            mainDeck.Events.Add(new Event("Ainding Timmy"));
            mainDeck.Events.Add(new Event("Binding Timmy"));
            Decks.Add(mainDeck);
            Decks.Add(undeadDeck);
            SelectedDeck = mainDeck;
            SelectedEvent = mainDeck.Events[0];
        }
        private void InitialiseInRealMode()
        {
            if (Directory.Exists(DataStorage.StorageDirectory))
            {
                foreach (string directory in Directory.EnumerateDirectories(DataStorage.StorageDirectory))
                {
                    string subDirectory = directory.Replace(DataStorage.StorageDirectory, "");
                    Deck tempDeck = new Deck(subDirectory);
                    foreach (string file in Directory.EnumerateFiles(directory))
                    {
                        Event tempEvent = DataStorage.LoadEventFromXml(file);
                        tempDeck.Events.Add(tempEvent);
                    }
                    Decks.Add(tempDeck);
                }
            }
        }

        private bool DeckTitleContains(object obj)
        {
            Deck deck = obj as Deck;
            if (deck == null) return false;
            return (CultureInfo.CurrentCulture.CompareInfo.IndexOf(deck.Title, DeckFilter, CompareOptions.IgnoreCase) >= 0);
        }
        private bool EventTitleContains(object obj)
        {
            Event ev = obj as Event;
            if (ev == null) return false;
            return (CultureInfo.CurrentCulture.CompareInfo.IndexOf(ev.Title, EventFilter, CompareOptions.IgnoreCase) >= 0);
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
                EventsViewSource.Source = SelectedDeck.Events;
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
                IsEditEventVisible = SelectedEvent == null ? false : true;
            }
        }

        private bool isStatusbarVisible = true;
        public bool IsStatusbarVisible
        {
            get
            {
                return isStatusbarVisible;
            }
            set
            {
                Set(() => IsStatusbarVisible, ref isStatusbarVisible, value);
            }
        }

        private bool isEditEventVisible = false;
        public bool IsEditEventVisible
        {
            get
            {
                return isEditEventVisible;
            }
            set
            {
                Set(() => IsEditEventVisible, ref isEditEventVisible, value);
            }
        }

        private bool isSettingsFlyoutVisible = false;
        public bool IsSettingsFlyoutVisible
        {
            get
            {
                return isSettingsFlyoutVisible;
            }
            set
            {
                Set(() => IsSettingsFlyoutVisible, ref isSettingsFlyoutVisible, value);
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
                    DecksViewSource.View.Filter = new Predicate<object>(DeckTitleContains);
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
                    EventsViewSource.View.Filter = new Predicate<object>(EventTitleContains);
            }
        }

        private RelayCommand showSettingsCommand;
        public RelayCommand ShowSettingsCommand
        {
            get
            {
                return showSettingsCommand ?? (showSettingsCommand = new RelayCommand(ExecuteShowSettingsCommand));
            }
        }
        private void ExecuteShowSettingsCommand()
        {
            IsSettingsFlyoutVisible = !IsSettingsFlyoutVisible;
        }

        private RelayCommand addEventCommand;
        public RelayCommand AddEventCommand
        {
            get
            {
                return addEventCommand ?? (addEventCommand = new RelayCommand(ExecuteAddEventCommand));
            }
        }
        private void ExecuteAddEventCommand()
        {
            AddNewEvent();
        }
        private void AddNewEvent()
        {
            if (SelectedDeck != null)
            {
                
                Event tempEvent = new Event("Untitled");
                SelectedDeck.Events.Add(tempEvent);
                SelectedEvent = tempEvent;
                SaveSelectedEvent();
            }
        }

        private RelayCommand addDeckCommand;
        public RelayCommand AddDeckCommand
        {
            get
            {
                return addDeckCommand ?? (addDeckCommand = new RelayCommand(ExecuteAddDeckCommand));
            }
        }
        private void ExecuteAddDeckCommand()
        {
            AddNewDeck();
        }
        private void AddNewDeck()
        {
            if (Decks != null)
            {
                Deck tempDeck = new Deck("Untitled");
                Decks.Add(tempDeck);
                SelectedDeck = tempDeck;
            }
        }

        private RelayCommand saveEventCommand;
        public RelayCommand SaveEventCommand
        {
            get
            {
                return saveEventCommand ?? (saveEventCommand = new RelayCommand(ExecuteSaveEventCommand));
            }
        }
        private void ExecuteSaveEventCommand()
        {
            SaveSelectedEvent();
        }
        private void SaveSelectedEvent()
        {
            if (SelectedEvent != null)
            {
                DataStorage.SaveEventToXml(SelectedEvent, SelectedDeck);
            }
        }

    }
}