using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace Eventful.Model
{
    public class Option : ViewModelBase
    {
        public Option()
        {
        }

        public Option(int parentId)
        {
            ResultingScreen = new Screen(parentId + 1);
        }

        private int screenId = 0;
        public int ScreenId
        {
            get
            {
                return screenId;
            }
            set
            {
                Set(() => ScreenId, ref screenId, value);
            }
        }

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
