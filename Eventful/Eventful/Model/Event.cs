using GalaSoft.MvvmLight;
using System;
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

        private ObservableCollection<Screen> screens = new ObservableCollection<Screen>();
        public ObservableCollection<Screen> Screens
        {
            get
            {
                return screens;
            }
            set
            {
                Set(() => Screens, ref screens, value);
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

        private bool isChanged;
        public bool IsChanged
        {
            get
            {
                return isChanged;
            }
            set
            {
                Set(() => IsChanged, ref isChanged, value);
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
