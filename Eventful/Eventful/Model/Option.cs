using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace Eventful.Model
{
    public class Option : ViewModelBase
    {
        private Screen resultingScreen = new Screen();
        public Screen ResultingScreen
        {
            get
            {
                return resultingScreen;
            }
            set
            {
                Set(() => ResultingScreen, ref resultingScreen, value);
            }
        }

        private string text = "Enter text here";
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

        private ObservableCollection<int> actions = new ObservableCollection<int>();
        public ObservableCollection<int> Actions
        {
            get
            {
                return actions;
            }
            set
            {
                Set(() => Actions, ref actions, value);
            }
        }


    }
}
