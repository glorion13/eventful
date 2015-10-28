using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows;

namespace Eventful.Model
{
    public class Option : ViewModelBase
    {
        public Option()
        {
        }

        private Point hotspot;
        public Point Hotspot
        {
            get
            {
                return hotspot;
            }
            set
            {
                Set(() => Hotspot, ref hotspot, value);
                string test = "";
            }
        }

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

    }
}
