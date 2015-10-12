using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace Eventful.Model
{
    public class Screen : ViewModelBase
    {
        public Screen()
        {
        }

        public Screen(int parentId)
        {
            Id = parentId + 1;
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

        private int id = 0;
        public int Id
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
    }
}
