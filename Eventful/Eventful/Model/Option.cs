using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Xml.Serialization;

namespace Eventful.Model
{
    public class Option : ViewModelBase
    {
        public Option()
        {
        }

        [XmlIgnore]
        private Screen source = null;
        [XmlIgnore]
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

        [XmlIgnore]
        private Screen target = null;
        [XmlIgnore]
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

        private Guid targetId = Guid.Empty;
        public Guid TargetId
        {
            get
            {
                return targetId;
            }
            set
            {
                Set(() => TargetId, ref targetId, value);
            }
        }

        public void UpdateTargetFromId()
        {
            Target = Source.ParentEvent.Screens.SingleOrDefault(screen => screen.Id == TargetId);
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
            if (Source == null)
                IsVisible = false;
            else if (Target == null)
                IsVisible = false;
            else
            {
                IsVisible = true;
                UpdateSource();
            }
        }

        public void UpdateSource()
        {
            SourceX = Source.X + ((Source.GetOptionIndex(this) + 1) * Width) - (Width / 2);
            SourceY = Source.Y + Height + Source.Height;
        }

        private double width = 25;
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

        private double height = 25;
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

        [XmlIgnore]
        public Screen Hotspot = new Screen();

        private RelayCommand<DragDeltaEventArgs> dragDeltaCommand;
        public RelayCommand<DragDeltaEventArgs> DragDeltaCommand
        {
            get
            {
                return dragDeltaCommand ?? (dragDeltaCommand = new RelayCommand<DragDeltaEventArgs>(ExecuteDragDeltaCommand));
            }
        }
        private void ExecuteDragDeltaCommand(DragDeltaEventArgs args)
        {
            Hotspot.InputX = SourceX;
            Hotspot.InputY = SourceY;
            Hotspot.InputX += args.HorizontalChange;
            Hotspot.InputY += args.VerticalChange;
            TargetId = Guid.Empty;
            Target = Hotspot;

            foreach (Screen screen in Source.ParentEvent.Screens)
            {
                if (!screen.Options.Contains(this))
                {
                    if ((Hotspot.InputX > screen.X) && (Hotspot.InputX < screen.X + screen.Width) && (Hotspot.InputY > screen.Y) && (Hotspot.InputY < screen.Y + screen.Height))
                    {
                        TargetId = screen.Id;
                        Target = screen;
                        break;
                    }
                }
            }

            args.Handled = true;
        }

        private RelayCommand<DragCompletedEventArgs> dropCommand;
        public RelayCommand<DragCompletedEventArgs> DropCommand
        {
            get
            {
                return dropCommand ?? (dropCommand = new RelayCommand<DragCompletedEventArgs>(ExecuteDropCommand));
            }
        }
        private void ExecuteDropCommand(DragCompletedEventArgs args)
        {
            UpdateTargetFromId();
        }

        private bool isVisible = false;
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                Set(() => IsVisible, ref isVisible, value);
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
