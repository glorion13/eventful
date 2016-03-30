using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventful.ViewModel
{
    public class Tag : ViewModelBase
    {
        public Tag(string title)
        {
            Title = title;
        }

        private HashSet<Event> events;
        public HashSet<Event> Events
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

        public bool Contains(Event e)
        {
            return Events.Contains(e);
        }

        public void AddEvent(Event e)
        {
            Events.Add(e);
            RaisePropertyChanged("Events");
        }

        public void RemoveEvent(Event e)
        {
            Events.Remove(e);
        }

    }
}
