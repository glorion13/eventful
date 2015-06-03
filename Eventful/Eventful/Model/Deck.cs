using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventful.Model
{
    public class Deck : ViewModelBase
    {
        public Deck()
        {
            Events = new ObservableCollection<Event>();
        }
        public Deck(string title)
        {
            Events = new ObservableCollection<Event>();
            Title = title;
        }
        public Deck(Deck deck)
        {
            Events = new ObservableCollection<Event>(deck.Events);
            Title = deck.Title;
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

        private ObservableCollection<Event> events;
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
    }
}
