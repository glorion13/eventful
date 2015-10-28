using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Eventful.Model
{
    public class Screen : ViewModelBase
    {
        public Screen()
        {
        }

        public Screen(Event parentEvent)
        {
            ParentEvent = parentEvent;
        }

        private string title = "Untitled Screen";
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                Set(() => Title, ref title, value);
                if (ParentEvent != null)
                    ParentEvent.IsChanged = true;
            }
        }

        private int x;
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                Set(() => X, ref x, value);
                if (ParentEvent != null)
                    ParentEvent.IsChanged = true;
            }
        }

        private int y;
        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                Set(() => Y, ref y, value);
                if (ParentEvent != null)
                    ParentEvent.IsChanged = true;
            }
        }

        private string text = "<EventBody>\n\n</EventBody>";
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                Set(() => Text, ref text, value);
                if (ParentEvent != null)
                    ParentEvent.IsChanged = true;
            }
        }

        [XmlIgnore]
        private Event parentEvent;
        [XmlIgnore]
        public Event ParentEvent
        {
            get
            {
                return parentEvent;
            }
            set
            {
                Set(() => ParentEvent, ref parentEvent, value);
            }
        }

        private ObservableCollection<Option> options = new ObservableCollection<Option>();
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
        }

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                Set(() => IsSelected, ref isSelected, value);
                MessengerInstance.Send<Screen>(this);
            }
        }

    }
}
