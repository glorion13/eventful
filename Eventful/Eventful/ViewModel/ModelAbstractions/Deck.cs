using Eventful.Service;
using Eventful.Service.Auxiliary;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;

namespace Eventful.ViewModel
{
    public class Deck : ViewModelBase
    {
        public Deck()
        {
        }
        public Deck(string title)
        {
            Title = title;
        }
        public Deck(Deck deck)
        {
            Events = new ObservableCollection<Event>(deck.Events);
            Title = deck.Title;
            foreach (Event ev in deck.Events)
                Events.Add(ev);
        }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                Set(() => Title, ref title, value);
            }
        }

        private ObservableCollection<Event> events = new ObservableCollection<Event>();
        public ObservableCollection<Event> Events
        {
            get
            {
                return events;
            }
            set
            {
                Set(() => Events, ref events, value);
            }
        }

        public void AddEvent(string eventName)
        {
            Event tempEvent = new Event(eventName);
            Events.Add(tempEvent);
            //DataStorage.SaveEventToDisk(tempEvent, this);
        }
        public void AddEvent(Event ev)
        {
            Event tempEvent = new Event(ev);
            Events.Add(tempEvent);
            //DataStorage.SaveEventToDisk(tempEvent, this);
        }

        public void RemoveEvent(Event ev)
        {
            Events.Remove(ev);
            DataStorage.DeleteEvent(ev, this);
        }

        private RelayCommand addEventCommand;
        public RelayCommand AddEventCommand
        {
            get
            {
                return addEventCommand ?? (addEventCommand = new RelayCommand(ExecuteAddEventCommand));
            }
        }
        private async void ExecuteAddEventCommand()
        {
            string dialogResult = await MessageWindowsViewModel.ShowOkCancelInput("Create New Event", "What is the title of the new event? You can always change it later.");
            if (dialogResult == null)
            {
            }
            else if (StringChecker.IsFilenameValid(dialogResult))
            {
                AddEvent(dialogResult);
                //SelectedEvent = SelectedDeck.Events.Last();
            }
            else
            {
                await MessageWindowsViewModel.ShowOkMessage("Create New Event", "A title cannot be empty, nor contain any of the following characters: \n \\ / : * ? \" < > |");
                ExecuteAddEventCommand();
            }
        }

        private RelayCommand<Event> removeEventCommand;
        public RelayCommand<Event> RemoveEventCommand
        {
            get
            {
                return removeEventCommand ?? (removeEventCommand = new RelayCommand<Event>(ExecuteRemoveEventCommand));
            }
        }

        private async void ExecuteRemoveEventCommand(Event ev)
        {
            bool dialogResult = await MessageWindowsViewModel.ShowOkCancelMessage("Confirm Event Deletion", $"Do you want to delete the event \"{ev.Title}\"?");
            if (dialogResult)
                RemoveEvent(ev);
        }

    }
}
