using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Linq;
using WPFFolderBrowser;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using System.Collections;
using System.Threading.Tasks;
using Eventful.Service.Auxiliary;
using Eventful.Service;

namespace Eventful.ViewModel
{
    public class MainViewModel : ViewModelBase, IDropTarget
    {
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
            }
            else
            {
                InitialiseViewSources();
                InitialiseAuthor();
                InitialiseStorageDirectory();
                InitialiseMessengerService();
                LoadDeckMappingsFromDisk();
                InitialiseAutocompletion();
            }
        }

        private void InitialiseViewSources()
        {
            DeckFilter = "";
            DecksViewSource = new CollectionViewSource();
            DecksViewSource.Source = Decks;
            EventsViewSource = new CollectionViewSource();
            DecksViewSource.View.SortDescriptions.Add(new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));
            ScreensViewSource = new CollectionViewSource();
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
        private void InitialiseMessengerService()
        {
        }
        private void InitialiseAutocompletion()
        {
            AutocompleteTrees.Add("var", Variables);
            AutocompleteTrees.Add("tag", Tags);
        }

        private void LoadDeckMappingsFromDisk()
        {
            Decks.Clear();
            foreach (Deck deck in DataStorage.LoadAllDeckMappingsFromDisk())
                Decks.Add(deck);
        }
        private async Task LoadSelectedDeckEventMappingsFromDisk()
        {
            SelectedDeck.Events.Clear();
            foreach (Event ev in await DataStorage.LoadDeckEventsFromDisk(SelectedDeck))
                SelectedDeck.Events.Add(ev);
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
        private bool ScreenTitleContains(object obj)
        {
            Screen screen = obj as Screen;
            if (screen == null) return false;
            return (CultureInfo.CurrentCulture.CompareInfo.IndexOf(screen.Title, ScreenFilter, CompareOptions.IgnoreCase) >= 0);
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

        private string screenFilter;
        public string ScreenFilter
        {
            get
            {
                return screenFilter;
            }
            set
            {
                Set(() => ScreenFilter, ref screenFilter, value);
                if (ScreensViewSource != null)
                {
                    ScreensViewSource.View.Filter = new Predicate<object>(ScreenTitleContains);
                    InitialiseConnections();
                }
            }
        }

        private ObservableCollection<Deck> decks = new ObservableCollection<Deck>();
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
        public CollectionViewSource ScreensViewSource { get; set; }

        private string author = Environment.UserName;
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
            string dialogResult = await MessageWindowsViewModel.ShowOkCancelInput("Change Author Name", text);
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
                DataStorage.StorageDirectory = StorageDirectory;
                Properties.Settings.Default["StorageDirectory"] = StorageDirectory;
                Properties.Settings.Default.Save();
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
                LoadDeckMappingsFromDisk();
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
                SelectedEvent = null;
                SelectedScreen = null;
                IsRemoveDeckButtonEnabled = !(SelectedDeck == null);
                IsAddEventButtonEnabled = !(SelectedDeck == null);
                EventsViewSource.Source = SelectedDeck == null ? new ObservableCollection<Event>() : SelectedDeck.Events;
                EventsViewSource.View.SortDescriptions.Add(new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));
                if (SelectedDeck != null)
                    LoadSelectedDeckEventMappingsFromDisk();
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
                SelectedScreen = null;
                IsRemoveEventButtonEnabled = !(SelectedEvent == null);
                IsAddScreenButtonEnabled = !(SelectedEvent == null);
                ScreensViewSource.Source = SelectedEvent == null ? new ObservableCollection<Screen>() : SelectedEvent.Screens;
                ScreensViewSource.View.SortDescriptions.Add(new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));
                InitialiseConnections();
            }
        }

        private Screen selectedScreen;
        public Screen SelectedScreen
        {
            get
            {
                return selectedScreen;
            }
            set
            {
                Set(() => SelectedScreen, ref selectedScreen, value);
                SelectedOption = null;
                IsRemoveScreenButtonEnabled = !(SelectedScreen == null);
            }
        }

        private Option selectedOption;
        public Option SelectedOption
        {
            get
            {
                return selectedOption;
            }
            set
            {
                Set(() => SelectedOption, ref selectedOption, value);
                IsRemoveOptionButtonEnabled = SelectedOption == null ? false : true;                
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            Event sourceItem = dropInfo.Data as Event;
            Deck targetItem = dropInfo.TargetItem as Deck;

            if (sourceItem != null && targetItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }
        public void Drop(IDropInfo dropInfo)
        {
            Event sourceItem = dropInfo.Data as Event;
            Deck targetItem = dropInfo.TargetItem as Deck;
            MoveEventToNewDeck(sourceItem, SelectedDeck, targetItem);
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
                string dialogResult = await MessageWindowsViewModel.ShowOkCancelInput("Create New Deck", text);
                if (dialogResult == null)
                {
                }
                else if (Decks.Any(d => String.Equals(d.Title, dialogResult, StringComparison.OrdinalIgnoreCase)))
                    AddNewDeck(String.Concat("A deck with the title \"", dialogResult, "\" already exists. Please enter a different title."));
                else if (!StringChecker.FilenameStringCheck(dialogResult))
                    AddNewDeck(filenameErrorMessage);
                else
                {
                    Deck tempDeck = new Deck(dialogResult);
                    CreateDeck(tempDeck);
                }
            }
        }
        private async void CreateDeck(Deck deck)
        {
            bool success = DataStorage.SaveDeckToDisk(deck);
            if (success)
            {
                Decks.Add(deck);
                SelectedDeck = deck;
            }
            else
            {
                await MessageWindowsViewModel.ShowOkMessage("Couldn't Save Deck", "The deck was not saved successfully. Try again later and ensure the save folder is accessible.");
            }
        }

        private RelayCommand addScreenCommand;
        public RelayCommand AddScreenCommand
        {
            get
            {
                return addScreenCommand ?? (addScreenCommand = new RelayCommand(ExecuteAddScreenCommand));
            }
        }
        private async void ExecuteAddScreenCommand()
        {
            if (SelectedEvent != null)
            {
                string dialogResult = await MessageWindowsViewModel.ShowOkCancelInput("Create New Screen", "What is the title of the new screen? You can always change it later.");
                if (dialogResult == null)
                {
                }
                else
                {
                    SelectedEvent.AddScreen();
                    Screen newScreen = SelectedEvent.Screens.Last();
                    newScreen.Title = dialogResult;
                    InitialiseConnections();
                    ExecuteSelectScreenCommand(newScreen);
                }
            }
        }

        private RelayCommand<Screen> removeScreenCommand;
        public RelayCommand<Screen> RemoveScreenCommand
        {
            get
            {
                return removeScreenCommand ?? (removeScreenCommand = new RelayCommand<Screen>(ExecuteRemoveScreenCommand));
            }
        }
        private async void ExecuteRemoveScreenCommand(Screen screen)
        {
            if (screen == null) return;
            bool dialogResult = await MessageWindowsViewModel.ShowOkCancelMessage("Confirm Screen Deletion", $"Do you want to delete the screen {screen.Title}?");
            if (dialogResult)
                RemoveScreen(screen);
        }
        private void RemoveScreen(Screen screen)
        {
            SelectedEvent.Screens.Remove(screen);
            InitialiseConnections();
            if (SelectedScreen == screen)
                SelectedScreen = null;
        }

        private bool isAddScreenButtonEnabled = false;
        public bool IsAddScreenButtonEnabled
        {
            get
            {
                return isAddScreenButtonEnabled;
            }
            set
            {
                Set(() => IsAddScreenButtonEnabled, ref isAddScreenButtonEnabled, value);
            }
        }
        private bool isRemoveScreenButtonEnabled = false;
        public bool IsRemoveScreenButtonEnabled
        {
            get
            {
                return isRemoveScreenButtonEnabled;
            }
            set
            {
                Set(() => IsRemoveScreenButtonEnabled, ref isRemoveScreenButtonEnabled, value);
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
                string dialogResult = await MessageWindowsViewModel.ShowOkCancelInput("Create New Event", text);
                if (dialogResult == null)
                {
                }
                else if (StringChecker.FilenameStringCheck(dialogResult))
                {
                    Event tempEvent = new Event(dialogResult);
                    AddEventToDeck(tempEvent, SelectedDeck);
                    SelectedEvent = tempEvent;
                }
                else
                {
                    AddNewEvent(filenameErrorMessage);
                }
            }
        }
        private void AddEventToDeck(Event ev, Deck deck)
        {
            deck.Events.Add(ev);
            SaveEvent(ev, deck);
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
            if (SelectedDeck == null) return;
            bool dialogResult = await MessageWindowsViewModel.ShowOkCancelMessage("Confirm Deck Deletion", String.Concat("Do you want to delete the deck \"", SelectedDeck.Title, "\"?"));
            if (dialogResult)
                RemoveDeck(SelectedDeck);
        }
        private void RemoveDeck(Deck deck)
        {
            if (deck != null)
            {
                if (DataStorage.DeleteDeck(deck))
                    Decks.Remove(deck);
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
            bool dialogResult = await MessageWindowsViewModel.ShowOkCancelMessage("Confirm Event Deletion", String.Concat("Do you want to delete the event \"", SelectedEvent.Title, "\"?"));
            if (dialogResult)
                RemoveEventFromDeck(SelectedEvent, SelectedDeck);
        }
        private void RemoveEventFromDeck(Event ev, Deck deck)
        {
            if (ev != null && deck != null)
            {
                if (DataStorage.DeleteEvent(ev, deck))
                    deck.Events.Remove(ev);
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
                string dialogResult = await MessageWindowsViewModel.ShowOkCancelInput("Change Deck Name", text);
                
                if (dialogResult == null)
                {
                }
                else if (dialogResult == SelectedDeck.Title)
                {
                }
                else if (Decks.Any(d => String.Equals(d.Title, dialogResult, StringComparison.OrdinalIgnoreCase)))
                    ChangeDeckName($"A deck with the title {dialogResult} already exists. Please enter a different title.");
                else if (!StringChecker.FilenameStringCheck(dialogResult))
                {
                    ChangeDeckName(filenameErrorMessage);
                }
                else
                {
                    if (DataStorage.RenameDeck(SelectedDeck, dialogResult))
                        SelectedDeck.Title = dialogResult;
                }
            }
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
                string dialogResult = await MessageWindowsViewModel.ShowOkCancelInput("Change Event Name", text);
                if (dialogResult == null)
                {
                }
                else if (dialogResult == SelectedEvent.Title)
                {
                }
                else if (StringChecker.FilenameStringCheck(dialogResult))
                {
                    if (DataStorage.RenameEvent(SelectedEvent, SelectedDeck, dialogResult))
                    {
                        SelectedEvent.Title = dialogResult;
                        SelectedEvent.IsChanged = false;
                    }
                }
                else
                    ChangeEventName(filenameErrorMessage);
            }
        }

        private RelayCommand<Screen> changeScreenNameCommand;
        public RelayCommand<Screen> ChangeScreenNameCommand
        {
            get
            {
                return changeScreenNameCommand ?? (changeScreenNameCommand = new RelayCommand<Screen>(ExecuteChangeScreenNameCommand));
            }
        }
        private async void ExecuteChangeScreenNameCommand(Screen screen)
        {
            string text = "What is the new title of the screen?";
            if (screen != null)
            {
                string dialogResult = await MessageWindowsViewModel.ShowOkCancelInput("Change Screen Name", text);
                if (dialogResult == null)
                {
                }
                else if (dialogResult == screen.Title)
                {
                }
                else
                {
                    screen.Title = dialogResult;
                }
            }
        }

        private string filenameErrorMessage = "A title cannot be empty, nor contain any of the following characters: \n \\ / : * ? \" < > |";

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
                bool success = DataStorage.SaveEventToDisk(ev, deck);
                if (success)
                    ev.IsChanged = false;
                else
                    await MessageWindowsViewModel.ShowOkMessage("Couldn't Save Event", "The event was not saved successfully. Try again later and ensure the save folder is accessible.");
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
                    bool dialogResult = await MessageWindowsViewModel.ShowOkCancelMessage("Sync Data", "You haven't saved your changes yet. If you sync they will be lost. Are you sure you want to sync?");
                    if (dialogResult)
                        SyncAndKeepFocusOnSelectedElements();
                }
                else
                {
                    SyncAndKeepFocusOnSelectedElements();
                }
            }
            else
            {
                LoadDeckMappingsFromDisk();
            }
        }
        private void SyncAndKeepFocusOnSelectedElements()
        {
            string selectedDeckId = SelectedDeck?.Title;
            string selectedEventId = SelectedEvent?.Id.ToString();
            string selectedScreenId = SelectedScreen?.Id.ToString();

            SelectedDeck = null;
            SelectedEvent = null;
            SelectedScreen = null;

            LoadDeckMappingsFromDisk();
            SelectedDeck = Decks.SingleOrDefault(d => d.Title == selectedDeckId);
            if (SelectedDeck == null) return;
            
            SelectedEvent = SelectedDeck.Events.SingleOrDefault(e => e.Id.ToString() == selectedEventId);
            if (SelectedEvent == null) return;

            SelectedScreen = SelectedEvent.Screens.SingleOrDefault(s => s.Id.ToString() == selectedScreenId);
        }

        private RelayCommand<Screen> duplicateScreenCommand;
        public RelayCommand<Screen> DuplicateScreenCommand
        {
            get
            {
                return duplicateScreenCommand ?? (duplicateScreenCommand = new RelayCommand<Screen>(ExecuteDuplicateScreenCommand));
            }
        }
        private void ExecuteDuplicateScreenCommand(Screen screen)
        {
            if (screen == null) return;
            Screen duplicateScreen = new Screen(screen);
            duplicateScreen.Title += " Copy";
            SelectedEvent.AddScreen(duplicateScreen);
            ExecuteSelectScreenCommand(SelectedEvent.Screens.Last());
            InitialiseConnections();
        }

        private RelayCommand duplicateEventCommand;
        public RelayCommand DuplicateEventCommand
        {
            get
            {
                return duplicateEventCommand ?? (duplicateEventCommand = new RelayCommand(ExecuteDuplicateEventCommand));
            }
        }
        private void ExecuteDuplicateEventCommand()
        {
            if (SelectedEvent == null) return;
            Event duplicateEvent = new Event(SelectedEvent);
            duplicateEvent.Title += " Copy";
            while (SelectedDeck.Events.Any(e => e.Title == duplicateEvent.Title))
                duplicateEvent.Title += " Copy";

            /*for (int i = 0; i < duplicateEvent.Screens.Count; i++)
            {
                for (int j = 0; j < duplicateEvent.Screens[i].Options.Count; j++)
                {
                    duplicateEvent.Screens[i].Options[j].TargetId = SelectedEvent.Screens[i].Options[j].TargetId;
                    duplicateEvent.Screens[i].Options[j].UpdateTargetFromId();
                }
            }*/
            // TO-DO need to somehow duplicate files directly, and then just refresh the IDs

            AddEventToDeck(duplicateEvent, SelectedDeck);
            SelectedEvent = duplicateEvent;
        }

        private RelayCommand duplicateDeckCommand;
        public RelayCommand DuplicateDeckCommand
        {
            get
            {
                return duplicateDeckCommand ?? (duplicateDeckCommand = new RelayCommand(ExecuteDuplicateDeckCommand));
            }
        }
        private void ExecuteDuplicateDeckCommand()
        {
            if (SelectedDeck == null) return;
            Deck duplicateDeck = new Deck(SelectedDeck);
            duplicateDeck.Title += " Copy";
            while (Decks.Any(d => d.Title == duplicateDeck.Title))
                duplicateDeck.Title += " Copy";
            CreateDeck(duplicateDeck);
            SelectedDeck = duplicateDeck;
        }

        private RelayCommand<SelectionChangedEventArgs> moveEventToDeckCommand;
        public RelayCommand<SelectionChangedEventArgs> MoveEventToDeckCommand
        {
            get
            {
                return moveEventToDeckCommand ?? (moveEventToDeckCommand = new RelayCommand<SelectionChangedEventArgs>(ExecuteMoveEventToDeckCommand));
            }
        }
        private void ExecuteMoveEventToDeckCommand(SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0)
            {
                Deck newDeck = args.AddedItems[0] as Deck;
                Event currentEvent = SelectedEvent;
                if (SelectedEvent != null && SelectedDeck != null && newDeck != null)
                {
                    if (newDeck != SelectedDeck)
                    {
                        MoveEventToNewDeck(SelectedEvent, SelectedDeck, newDeck);
                        SelectedDeck = newDeck;
                        SelectedEvent = currentEvent;
                    }
                }
            }
        }
        private void MoveEventToNewDeck(Event ev, Deck oldDeck, Deck newDeck)
        {
            AddEventToDeck(ev, newDeck);
            RemoveEventFromDeck(ev, oldDeck);
        }

        private RelayCommand openNewEventWindowCommand;
        public RelayCommand OpenNewEventWindowCommand
        {
            get
            {
                return openNewEventWindowCommand ?? (openNewEventWindowCommand = new RelayCommand(ExecuteOpenNewEventWindowCommand));
            }
        }
        private void ExecuteOpenNewEventWindowCommand()
        {
            if (SelectedEvent == null) return;
            EditEventViewModel vm = new EditEventViewModel();
            vm.SelectedEvent = SelectedEvent;
            vm.SelectedDeck = SelectedDeck;
            MessengerInstance.Send(vm);
        }

        private RelayCommand openTagLibraryCommand;
        public RelayCommand OpenTagLibraryCommand
        {
            get
            {
                return openTagLibraryCommand ?? (openTagLibraryCommand = new RelayCommand(ExecuteOpenTagLibraryCommand));
            }
        }
        private void ExecuteOpenTagLibraryCommand()
        {
            TagLibraryViewModel vm = new TagLibraryViewModel();
            MessengerInstance.Send(vm);
        }

        private RelayCommand openVariableLibraryCommand;
        public RelayCommand OpenVariableLibraryCommand
        {
            get
            {
                return openVariableLibraryCommand ?? (openVariableLibraryCommand = new RelayCommand(ExecuteOpenVariableLibraryCommand));
            }
        }
        private void ExecuteOpenVariableLibraryCommand()
        {
            VariableLibraryViewModel vm = new VariableLibraryViewModel();
            MessengerInstance.Send(vm);
            MessengerInstance.Send<ObservableCollection<Variable>>(Variables);
        }

        private ObservableCollection<Variable> tags = new ObservableCollection<Variable>();
        public ObservableCollection<Variable> Tags
        {
            get
            {
                return tags;
            }
            set
            {
                Set(() => Tags, ref tags, value);
            }
        }

        private ObservableCollection<Variable> variables = new ObservableCollection<Variable>();
        public ObservableCollection<Variable> Variables
        {
            get
            {
                return variables;
            }
            set
            {
                Set(() => Variables, ref variables, value);
            }
        }

        private Hashtable autocompleteTrees = new Hashtable();
        public Hashtable AutocompleteTrees
        {
            get
            {
                return autocompleteTrees;
            }
            set
            {
                Set(() => AutocompleteTrees, ref autocompleteTrees, value);
            }
        }

        private string selectedText;
        public string SelectedText
        {
            get
            {
                return selectedText;
            }
            set
            {
                Set(() => SelectedText, ref selectedText, value);
            }
        }

        private RelayCommand addOptionCommand;
        public RelayCommand AddOptionCommand
        {
            get
            {
                return addOptionCommand ?? (addOptionCommand = new RelayCommand(ExecuteAddOptionCommand));
            }
        }
        private void ExecuteAddOptionCommand()
        {
            if (SelectedScreen == null) return;
            SelectedScreen.AddOption();
            InitialiseConnections();
            SelectedOption = SelectedScreen.Options.Last();
        }

        private bool isRemoveOptionButtonEnabled = false;
        public bool IsRemoveOptionButtonEnabled
        {
            get
            {
                return isRemoveOptionButtonEnabled;
            }
            set
            {
                Set(() => IsRemoveOptionButtonEnabled, ref isRemoveOptionButtonEnabled, value);
            }
        }
        private RelayCommand removeOptionCommand;
        public RelayCommand RemoveOptionCommand
        {
            get
            {
                return removeOptionCommand ?? (removeOptionCommand = new RelayCommand(ExecuteRemoveOptionCommand));
            }
        }
        private void ExecuteRemoveOptionCommand()
        {
            if (SelectedOption == null) return;
            if (SelectedScreen == null) return;

            foreach (Option option in SelectedScreen.Options)
                if (option.Index > SelectedOption.Index)
                    option.Index--;

            int index = SelectedOption.Index;

            SelectedScreen.Options.Remove(SelectedOption);

            if (index >= 2)
                SelectedOption = SelectedScreen.Options[index - 2];
            else if (SelectedScreen.Options.Count > 0)
                SelectedOption = SelectedScreen.Options[0];

            if (SelectedScreen.ParentEvent != null)
                SelectedScreen.ParentEvent.IsChanged = true;
            InitialiseConnections();
        }

        private RelayCommand moveOptionUpCommand;
        public RelayCommand MoveOptionUpCommand
        {
            get
            {
                return moveOptionUpCommand ?? (moveOptionUpCommand = new RelayCommand(ExecuteMoveOptionUpCommand));
            }
        }
        private void ExecuteMoveOptionUpCommand()
        {
            if (SelectedOption == null) return;
            if (SelectedScreen == null) return;
            if (SelectedOption.Index == 1) return;
            Option optionAbove = SelectedScreen.Options.FirstOrDefault(opt => opt.Index == SelectedOption.Index - 1);
            optionAbove.Index = SelectedOption.Index;
            SelectedOption.Index--;
            SelectedScreen.Options.Move(optionAbove.Index - 1, SelectedOption.Index - 1);
            SelectedScreen.Update();
            InitialiseConnections();
        }

        private RelayCommand moveOptionDownCommand;
        public RelayCommand MoveOptionDownCommand
        {
            get
            {
                return moveOptionDownCommand ?? (moveOptionDownCommand = new RelayCommand(ExecuteMoveOptionDownCommand));
            }
        }
        private void ExecuteMoveOptionDownCommand()
        {
            if (SelectedOption == null) return;
            if (SelectedScreen == null) return;
            if (SelectedOption.Index == SelectedScreen.Options.Count) return;
            Option optionAbove = SelectedScreen.Options.FirstOrDefault(opt => opt.Index == SelectedOption.Index + 1);
            optionAbove.Index = SelectedOption.Index;
            SelectedOption.Index++;
            SelectedScreen.Options.Move(optionAbove.Index - 1, SelectedOption.Index - 1);
            SelectedScreen.Update();
            InitialiseConnections();
        }

        private void InitialiseConnections()
        {
            Connections.Clear();
            if (SelectedEvent != null)
                foreach (Screen screen in ScreensViewSource.View)
                    foreach (Option option in screen.Options)
                        Connections.Add(option);
        }

        private ObservableCollection<Option> connections = new ObservableCollection<Option>();
        public ObservableCollection<Option> Connections
        {
            get
            {
                return connections;
            }
            set
            {
                Set(() => Connections, ref connections, value);
            }
        }

        private RelayCommand deselectSelectedScreenCommand;
        public RelayCommand DeselectSelectedScreenCommand
        {
            get
            {
                return deselectSelectedScreenCommand ?? (deselectSelectedScreenCommand = new RelayCommand(ExecuteDeselectSelectedScreenCommand));
            }
        }
        private void ExecuteDeselectSelectedScreenCommand()
        {
            if (SelectedScreen != null)
            {
                SelectedScreen.IsSelected = false;
                SelectedScreen = null;
            }
        }

        private RelayCommand<Screen> selectScreenCommand;
        public RelayCommand<Screen> SelectScreenCommand
        {
            get
            {
                return selectScreenCommand ?? (selectScreenCommand = new RelayCommand<Screen>(ExecuteSelectScreenCommand));
            }
        }
        private void ExecuteSelectScreenCommand(Screen screen)
        {
            if (SelectedDeck == null) return;
            if (SelectedEvent == null) return;

            ExecuteDeselectSelectedScreenCommand();
            SelectedScreen = screen;
            SelectedScreen.IsSelected = true;
        }
    }
}
