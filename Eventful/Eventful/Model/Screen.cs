using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace Eventful.Model
{
    public class Screen : ViewModelBase
    {
        public Screen()
        {
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
