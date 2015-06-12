using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventful.Model
{
    public class Tag : ViewModelBase
    {
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
