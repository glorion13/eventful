using Eventful.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Linq;
using WPFFolderBrowser;
using System.Windows.Controls;

namespace Eventful.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            InitialiseInAllModes();
            if (IsInDesignMode)
            {
                InitialiseInDesignMode();
            }
            else
            {
                InitialiseInRealMode();
            }
        }

        private void InitialiseInAllModes()
        {
            Decks = new ObservableCollection<Deck>();
            DeckFilter = "";
            DecksViewSource = new CollectionViewSource();
            DecksViewSource.Source = Decks;
            EventsViewSource = new CollectionViewSource();
            DecksViewSource.View.SortDescriptions.Add(new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));
            InitialiseAuthor();
            InitialiseStorageDirectory();
        }
        private void InitialiseInDesignMode()
        {
            Deck mainDeck = new Deck("Main");
            Deck undeadDeck = new Deck("Undead Army");
            mainDeck.Events.Add(new Event("Finding Timmy: A Bewildering Adventure"));
            Event ev = new Event("Cinding Timmy");
            ev.Text = "Hello world.";
            mainDeck.Events.Add(ev);
            mainDeck.Events.Add(new Event("Ainding Timmy"));
            mainDeck.Events.Add(new Event("Binding Timmy"));
            Decks.Add(mainDeck);
            Decks.Add(undeadDeck);
            SelectedDeck = mainDeck;
            SelectedEvent = mainDeck.Events[0];
        }
        private void InitialiseInRealMode()
        {
            LoadDecksFromDisk();
        }

        private void InitialiseAuthor()
        {
            try
            {
                string authorFromSettings = Properties.Settings.Default["Author"] as string;
                if (authorFromSettings != "")
                    Author = authorFromSettings;
                else
                {
                    Author = System.Environment.UserName;
                }
            }
            catch
            {
                Author = System.Environment.UserName;
            }
        }
        private void InitialiseStorageDirectory()
        {
            try
            {
                string storageDirectoryFromSettings = Properties.Settings.Default["StorageDirectory"] as string;
                if (storageDirectoryFromSettings != "")
                    StorageDirectory = storageDirectoryFromSettings;
                else
                    StorageDirectory = DataStorage.DefaultStorageDirectory;
            }
            catch
            {
                StorageDirectory = DataStorage.DefaultStorageDirectory;
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

        private string author = System.Environment.UserName;
        public string Author
        {
            get
            {
                return author;
            }
            set
            {
                Set(() => Author, ref author, value);
                Properties.Settings.Default["Author"] = Author;
                Properties.Settings.Default.Save();
            }
        }

        private string storageDirectory = DataStorage.DefaultStorageDirectory;
        public string StorageDirectory
        {
            get
            {
                return storageDirectory;
            }
            set
            {
                Set(() => StorageDirectory, ref storageDirectory, value);
                Properties.Settings.Default["StorageDirectory"] = StorageDirectory;
                Properties.Settings.Default.Save();
                //DataStorage.CreateStorageDirectory(StorageDirectory);
            }
        }

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
                IsRemoveDeckButtonEnabled = SelectedDeck == null ? false : true;
                IsAddEventButtonEnabled = SelectedDeck == null ? false : true;
                EventsViewSource.Source = SelectedDeck == null ? new ObservableCollection<Event>() : SelectedDeck.Events;
                EventsViewSource.View.SortDescriptions.Add(new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));
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
                IsRemoveEventButtonEnabled = SelectedEvent == null ? false : true;
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

        private bool isAddDeckButtonEnabled = true;
        public bool IsAddDeckButtonEnabled
        {
            get
            {
                return isAddDeckButtonEnabled;
            }
            set
            {
                Set(() => IsAddDeckButtonEnabled, ref isAddDeckButtonEnabled, value);
            }
        }

        private bool isAddEventButtonEnabled = false;
        public bool IsAddEventButtonEnabled
        {
            get
            {
                return isAddEventButtonEnabled;
            }
            set
            {
                Set(() => IsAddEventButtonEnabled, ref isAddEventButtonEnabled, value);
            }
        }

        private bool isRemoveDeckButtonEnabled = false;
        public bool IsRemoveDeckButtonEnabled
        {
            get
            {
                return isRemoveDeckButtonEnabled;
            }
            set
            {
                Set(() => IsRemoveDeckButtonEnabled, ref isRemoveDeckButtonEnabled, value);
            }
        }

        private bool isRemoveEventButtonEnabled = false;
        public bool IsRemoveEventButtonEnabled
        {
            get
            {
                return isRemoveEventButtonEnabled;
            }
            set
            {
                Set(() => IsRemoveEventButtonEnabled, ref isRemoveEventButtonEnabled, value);
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

        private RelayCommand changeEventNameCommand;
        public RelayCommand ChangeEventNameCommand
        {
            get
            {
                return changeEventNameCommand ?? (changeEventNameCommand = new RelayCommand(ExecuteChangeEventNameCommand));
            }
        }
        private void ExecuteChangeEventNameCommand()
        {
            ChangeEventName("What is the new title of the event?");
        }
        private async void ChangeEventName(string text)
        {
            if (SelectedEvent != null && SelectedDeck != null)
            {
                string dialogResult = await ShowOkCancelInput("Change Event Name", text);
                if (SelectedDeck.Events.Any(e => String.Equals(e.Title, dialogResult, StringComparison.OrdinalIgnoreCase)))
                {
                    ChangeEventName(String.Concat("An event with the title \"", dialogResult, "\" already exists in this deck. Please enter a different title."));
                }
                else if (dialogResult == null)
                {
                }
                else if (dialogResult == "")
                {
                    ChangeEventName("An event title cannot be empty.");
                }
                else
                {
                    DataStorage.DeleteEvent(SelectedEvent, SelectedDeck, StorageDirectory);
                    SelectedEvent.Title = dialogResult;
                    SaveEvent(SelectedEvent, SelectedDeck);
                }
            }
        }

        private RelayCommand changeDeckNameCommand;
        public RelayCommand ChangeDeckNameCommand
        {
            get
            {
                return changeDeckNameCommand ?? (changeDeckNameCommand = new RelayCommand(ExecuteChangeDeckNameCommand));
            }
        }
        private void ExecuteChangeDeckNameCommand()
        {
            ChangeDeckName("What is the new title of the deck?");
        }
        private async void ChangeDeckName(string text)
        {
            if (SelectedDeck != null)
            {
                string dialogResult = await ShowOkCancelInput("Change Deck Name", text);
                if (Decks.Any(d => String.Equals(d.Title, dialogResult, StringComparison.OrdinalIgnoreCase)))
                {
                    ChangeDeckName(String.Concat("A deck with the title \"", dialogResult, "\" already exists. Please enter a different title."));
                }
                else if (dialogResult == null)
                {
                }
                else if (dialogResult == "")
                {
                    ChangeDeckName("A deck title cannot be empty.");
                }
                else
                {
                    DataStorage.DeleteDeck(SelectedDeck, StorageDirectory);
                    SelectedDeck.Title = dialogResult;
                    DataStorage.SaveDeck(SelectedDeck, StorageDirectory);
                }
            }
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
            AddNewEvent("What is the title of the new event? You can always change it later.");
        }
        private async void AddNewEvent(string text)
        {
            if (SelectedDeck != null)
            {
                string dialogResult = await ShowOkCancelInput("Create New Event", text);
                /*if (SelectedDeck.Events.Any(e => String.Equals(e.Title, dialogResult, StringComparison.OrdinalIgnoreCase)))
                {
                    AddNewEvent(String.Concat("An event with the title \"", dialogResult, "\" already exists in this deck. Please enter a different title."));
                }*/
                if (dialogResult == null)
                {
                }
                else if (dialogResult == "")
                {
                    AddNewEvent("An event title cannot be empty.");
                }
                else
                {
                    Event tempEvent = new Event(dialogResult);
                    AddEventToDeck(tempEvent, SelectedDeck);
                    SelectedEvent = tempEvent;
                }
            }
        }
        private void AddEventToDeck(Event ev, Deck deck)
        {
            deck.Events.Add(ev);
            SaveEvent(ev, deck);
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
            AddNewDeck("What is the title of the new deck? You can always change it later.");
        }
        private async void AddNewDeck(string text)
        {
            if (Decks != null)
            {
                string dialogResult = await ShowOkCancelInput("Create New Deck", text);
                /*if (Decks.Any(e => String.Equals(e.Title, dialogResult, StringComparison.OrdinalIgnoreCase)))
                {
                    AddNewDeck(String.Concat("A deck with the title \"", dialogResult, "\" already exists. Please enter a different title."));
                }*/
                if (dialogResult == null)
                {
                }
                else if (dialogResult == "")
                {
                    AddNewDeck("A deck title cannot be empty.");
                }
                else
                {
                    Deck tempDeck = new Deck(dialogResult);
                    bool success = DataStorage.SaveDeck(tempDeck, StorageDirectory);
                    if (success)
                    {
                        Decks.Add(tempDeck);
                        SelectedDeck = tempDeck;
                    }
                    else
                    {
                        await ShowOkMessage("Couldn't Save Deck", "The deck was not saved successfully. Try again later and ensure the save folder is accessible.");
                    }
                }
            }
        }

        private RelayCommand removeEventCommand;
        public RelayCommand RemoveEventCommand
        {
            get
            {
                return removeEventCommand ?? (removeEventCommand = new RelayCommand(ExecuteRemoveEventCommand));
            }
        }
        private async void ExecuteRemoveEventCommand()
        {
            bool dialogResult = await ShowOkCancelMessage("Confirm Event Deletion", String.Concat("Do you want to delete the event \"", SelectedEvent.Title, "\"?"));
            if (dialogResult)
                RemoveEventFromDeck(SelectedEvent, SelectedDeck);
        }
        private void RemoveEventFromDeck(Event ev, Deck deck)
        {
            if (ev != null && deck != null)
            {
                try
                {
                    DataStorage.DeleteEvent(ev, deck, StorageDirectory);
                    deck.Events.Remove(ev);
                }
                catch
                {

                }
            }
        }

        private RelayCommand removeDeckCommand;
        public RelayCommand RemoveDeckCommand
        {
            get
            {
                return removeDeckCommand ?? (removeDeckCommand = new RelayCommand(ExecuteRemoveDeckCommand));
            }
        }
        private async void ExecuteRemoveDeckCommand()
        {
            bool dialogResult = await ShowOkCancelMessage("Confirm Deck Deletion", String.Concat("Do you want to delete the deck \"", SelectedDeck.Title, "\"?"));
            if (dialogResult)
                RemoveDeck(SelectedDeck);
        }
        private void RemoveDeck(Deck deck)
        {
            if (deck != null)
            {
                try
                {
                    DataStorage.DeleteDeck(deck, StorageDirectory);
                    Decks.Remove(deck);
                }
                catch
                {
                }
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
            SaveEvent(SelectedEvent, SelectedDeck);
        }
        private async void SaveEvent(Event ev, Deck deck)
        {
            if (ev != null)
            {
                ev.Author = Author;
                ev.Date = DateTime.Now;
                bool success = DataStorage.SaveEventToXml(ev, deck, StorageDirectory);
                if (success)
                    ev.IsChanged = false;
                else
                    await ShowOkMessage("Couldn't Save Event", "The event was not saved successfully. Try again later and ensure the save folder is accessible.");
            }
        }

        private RelayCommand changeAuthorCommand;
        public RelayCommand ChangeAuthorCommand
        {
            get
            {
                return changeAuthorCommand ?? (changeAuthorCommand = new RelayCommand(ExecuteChangeAuthorCommand));
            }
        }
        private void ExecuteChangeAuthorCommand()
        {
            ChangeAuthor("What is your name?");
        }
        private async void ChangeAuthor(string text)
        {
            string dialogResult = await ShowOkCancelInput("Change Author Name", text);
            if (dialogResult == null)
            {
            }
            else if (dialogResult == "")
            {
                ChangeAuthor("Your name cannot be empty.");
            }
            else
            {
                Author = dialogResult;
            }
        }

        private RelayCommand browseStorageDirectoryCommand;
        public RelayCommand BrowseStorageDirectoryCommand
        {
            get
            {
                return browseStorageDirectoryCommand ?? (browseStorageDirectoryCommand = new RelayCommand(ExecuteBrowseStorageDirectoryCommand));
            }
        }
        private void ExecuteBrowseStorageDirectoryCommand()
        {
            WPFFolderBrowserDialog dialog = new WPFFolderBrowserDialog();
            dialog.Title = "Please select a folder";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                string[] pathTokens = dialog.FileName.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
                StorageDirectory = (pathTokens[pathTokens.Length - 1] == "Eventful") ? String.Concat(dialog.FileName, @"\") : String.Concat(dialog.FileName, @"\Eventful\");
                LoadDecksFromDisk();
            }
        }

        private RelayCommand syncDataCommand;
        public RelayCommand SyncDataCommand
        {
            get
            {
                return syncDataCommand ?? (syncDataCommand = new RelayCommand(ExecuteSyncDataCommand));
            }
        }
        private async void ExecuteSyncDataCommand()
        {
            if (SelectedEvent != null)
            {
                if (SelectedEvent.IsChanged)
                {
                    bool dialogResult = await ShowOkCancelMessage("Sync Data", "You haven't saved your changes yet. If you sync they will be lost. Are you sure you want to sync?");
                    if (dialogResult)
                    {
                        string deckTitle = SelectedDeck.Title;
                        string eventTitle = SelectedEvent.Title;
                        LoadDecksFromDisk();
                        SelectedDeck = Decks.FirstOrDefault(d => d.Title == deckTitle);
                        SelectedEvent = (SelectedDeck == null) ? null : SelectedDeck.Events.FirstOrDefault(e => e.Title == eventTitle);
                    }
                }
                else
                {
                    string deckTitle = SelectedDeck.Title;
                    string eventTitle = SelectedEvent.Title;
                    LoadDecksFromDisk();
                    SelectedDeck = Decks.FirstOrDefault(d => d.Title == deckTitle);
                    SelectedEvent = (SelectedDeck == null) ? null : SelectedDeck.Events.FirstOrDefault(e => e.Title == eventTitle);
                }
            }
            else
            {
                LoadDecksFromDisk();
            }
        }

        private RelayCommand<SelectionChangedEventArgs> moveEventToDeckCommand;
        public RelayCommand<SelectionChangedEventArgs> MoveEventToDeckCommand
        {
            get
            {
                return moveEventToDeckCommand ?? (moveEventToDeckCommand = new RelayCommand<SelectionChangedEventArgs>(ExecuteMoveEventToDeckCommand));
            }
        }
        private  void ExecuteMoveEventToDeckCommand(SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0)
            {
                Deck newDeck = args.AddedItems[0] as Deck;
                Event currentEvent = SelectedEvent;
                if (SelectedEvent != null && SelectedDeck != null && newDeck != null)
                {
                    if (newDeck != SelectedDeck)
                    {
                        /*if (newDeck.Events.Any(e => String.Equals(e.Title, SelectedEvent.Title, StringComparison.OrdinalIgnoreCase)))
                        {
                            SelectedDeck = newDeck;
                            SelectedEvent = newDeck.Events.FirstOrDefault(e => String.Equals(e.Title, currentEvent.Title, StringComparison.OrdinalIgnoreCase));
                            await ShowOkMessage("Move Event", String.Concat("An event named \"", SelectedEvent.Title, "\" already exists in the \"", newDeck.Title, "\" deck. Rename it and then try again."));
                        }
                        else
                        {*/
                        MoveEventToDeck(SelectedEvent, SelectedDeck, newDeck);
                        SelectedDeck = newDeck;
                        SelectedEvent = newDeck.Events.FirstOrDefault(e => String.Equals(e.Title, currentEvent.Title, StringComparison.OrdinalIgnoreCase));
                        //}
                    }
                }
            }
        }
        private void MoveEventToDeck(Event ev, Deck oldDeck, Deck newDeck)
        {
            AddEventToDeck(ev, newDeck);
            RemoveEventFromDeck(ev, oldDeck);
        }

        private void LoadDecksFromDisk()
        {
            Decks.Clear();
            if (Directory.Exists(StorageDirectory))
            {
                foreach (string directory in Directory.EnumerateDirectories(StorageDirectory))
                {
                    string subDirectory = directory.Replace(StorageDirectory, "");
                    if (subDirectory[0] != '.')
                    {
                        Deck tempDeck = new Deck(subDirectory);
                        foreach (string file in Directory.EnumerateFiles(directory, "*.event"))
                        {
                            Event tempEvent = DataStorage.LoadEventFromXml(file);
                            if (tempEvent != null)
                                tempDeck.Events.Add(tempEvent);
                        }
                        Decks.Add(tempDeck);
                    }
                }
            }
        }

        private async Task ShowOkMessage(string title, string body)
        {
            MetroWindow metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
            await metroWindow.ShowMessageAsync(title, body, MessageDialogStyle.Affirmative);
        }
        private async Task<bool> ShowOkCancelMessage(string title, string body)
        {
            MetroWindow metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
            MessageDialogResult dialogResult = await metroWindow.ShowMessageAsync(title, body, MessageDialogStyle.AffirmativeAndNegative);
            return dialogResult == MessageDialogResult.Affirmative;
        }
        private async Task<string> ShowOkCancelInput(string title, string body)
        {
            MetroWindow metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
            string dialogResult = await metroWindow.ShowInputAsync(title, body);
            return dialogResult;
        }

    }
}