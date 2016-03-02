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

        private Screen source;
        public Screen Source
        {
            get
            {
                return source;
            }
            set
            {
                Set(() => Source, ref source, value);
                Update();
            }
        }

        private Screen target;
        public Screen Target
        {
            get
            {
                return target;
            }
            set
            {
                Set(() => Target, ref target, value);
                Update();
            }
        }

        private double sourceX;
        public double SourceX
        {
            get
            {
                return sourceX;
            }
            set
            {
                Set(() => SourceX, ref sourceX, value);
            }
        }

        private double sourceY;
        public double SourceY
        {
            get
            {
                return sourceY;
            }
            set
            {
                Set(() => SourceY, ref sourceY, value);
            }
        }

        public void Update()
        {
            if (Source != null)
                UpdateSource();
        }

        public void UpdateSource()
        {
            SourceX = Source.X + (Width / 2) + (Source.GetOptionIndex(this) * Width);
            SourceY = Source.Y + (Height) + Source.Height;
        }

        private double width = 10;
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                Set(() => Width, ref width, value);
            }
        }

        private double height = 10;
        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                Set(() => Height, ref height, value);
            }
        }

        private int index;
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                Set(() => Index, ref index, value);
            }
        }

        /*private Screen resultingScreen = new Screen();
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
        }*/

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
