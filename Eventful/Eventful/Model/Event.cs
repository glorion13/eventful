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
            }
        }

        private string notes = "";
        public string Notes
        {
            get
            {
                return notes;
            }
            set
            {
                Set(() => Notes, ref notes, value);
            }
        }

    }
}
