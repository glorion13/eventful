using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Eventful.Model
{
    [Serializable]
    public class Event : ViewModelBase
    {
        public Event()
        {
        }
        public Event(string title)
        {
            Title = title;
        }
        public Event(Event ev)
        {
            Title = ev.Title;
            Author = ev.Author;
        }

        private Screen startingScreen = new Screen();
        public Screen StartingScreen
        {
            get
            {
                return startingScreen;
            }
            set
            {
                Set(() => StartingScreen, ref startingScreen, value);
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

        private Guid id = Guid.NewGuid();
        public Guid Id
        {
            get
            {
                return id;
            }
            set
            {
                Set(() => Id, ref id, value);
            }
        }

        private string author = "anonymous";
        public string Author
        {
            get
            {
                return author;
            }
            set
            {
                Set(() => Author, ref author, value);
            }
        }

        private DateTime date = DateTime.Now;
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                Set(() => Date, ref date, value);
            }
        }

    }
}
