using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Eventful.ViewModel
{
    [DataContract]
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
            foreach (Screen screen in ev.Screens)
                AddScreen(new Screen(screen), true);
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        [IgnoreDataMember]
        private bool isChanged = false;
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

        public void AddScreen(string screenName)
        {
            Screen screen = new Screen();
            screen.ParentEvent = this;
            screen.Title = screenName;
            screen.X = 10;
            screen.Y = 10;
            Screens.Add(screen);
        }
        public void AddScreen(Screen screen)
        {
            screen.ParentEvent = this;
            screen.X = 10;
            screen.Y = 10;
            Screens.Add(screen);
        }
        public void AddScreen(Screen screen, bool keepScreenPosition)
        {
            screen.ParentEvent = this;
            if (!keepScreenPosition)
            {
                screen.X = 10;
                screen.Y = 10;
            }
            Screens.Add(screen);
        }

        public void RemoveScreen(Screen screen)
        {
            Screens.Remove(screen);
        }
    }
}
