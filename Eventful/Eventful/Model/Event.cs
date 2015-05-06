using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eventful.Model
{
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
    }
}
