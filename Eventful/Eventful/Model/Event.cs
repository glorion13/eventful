using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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
            Text = ev.Text;
            Author = ev.Author;
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
                IsChanged = true;
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
                IsChanged = true;
            }
        }

        private string text = "";
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                Set(() => Text, ref text, value);
                IsChanged = true;
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

        /*private ObservableCollection<Option> options;
        public ObservableCollection<Option> Options
        {
            get
            {
                return options;
            }
            set
            {
                Set(() => Options, ref options, value);
            }
        }*/
    }
}
