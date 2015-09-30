using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace Eventful.Model
{
    public class Option : ViewModelBase
    {
        private Screen resultingScreen;
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

        private string text;
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

    }
}
