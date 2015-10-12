using GalaSoft.MvvmLight;

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
