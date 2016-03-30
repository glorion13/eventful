using Eventful.Service;
using GalaSoft.MvvmLight;
using System;
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
            DataStorage.SaveEventToDisk(tempEvent, this);
        }
        public void AddEvent(Event ev)
        {
            Event tempEvent = new Event(ev);
            Events.Add(tempEvent);
            DataStorage.SaveEventToDisk(tempEvent, this);
        }

        public void RemoveEvent(Event ev)
        {
            Events.Remove(ev);
            DataStorage.DeleteEvent(ev, this);
        }
    }
}
