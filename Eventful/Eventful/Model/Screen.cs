﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
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

        private double x = 0;
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                Set(() => X, ref x, value);
                UpdateInputPositions();
                UpdateOutputPositions();
            }
        }

        private double y = 0;
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                Set(() => Y, ref y, value);
                UpdateInputPositions();
                UpdateOutputPositions();
            }
        }

        private double inputX;
        public double InputX
        {
            get
            {
                return inputX;
            }
            set
            {
                Set(() => InputX, ref inputX, value);
            }
        }

        private double inputY;
        public double InputY
        {
            get
            {
                return inputY;
            }
            set
            {
                Set(() => InputY, ref inputY, value);
            }
        }

        private double width = 150;
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                Set(() => Width, ref width, value);
                UpdateInputPositions();
                UpdateOutputPositions();
            }
        }

        private double height = 80;
        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                Set(() => Height, ref height, value);
                UpdateInputPositions();
                UpdateOutputPositions();
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
            X += args.HorizontalChange;
            Y += args.VerticalChange;
        }

        private RelayCommand gotFocusCommand;
        public RelayCommand GotFocusCommand
        {
            get
            {
                return gotFocusCommand ?? (gotFocusCommand = new RelayCommand(ExecuteGotFocusCommand));
            }
        }
        private void ExecuteGotFocusCommand()
        {
            IsSelected = true;
        }

        private RelayCommand lostFocusCommand;
        public RelayCommand LostFocusCommand
        {
            get
            {
                return lostFocusCommand ?? (lostFocusCommand = new RelayCommand(ExecuteLostFocusCommand));
            }
        }
        private void ExecuteLostFocusCommand()
        {
            IsSelected = false;
        }

        public void UpdateInputPositions()
        {
            InputX = X;// + (Width / 2);
            InputY = Y;// + (Height / 2);
        }

        public void UpdateOutputPositions()
        {
            foreach (Option option in Options)
                option.Update();
        }

        public int GetOptionIndex(Option output)
        {
            return Options.IndexOf(output);
        }

        public void AddOutput()
        {
            //NodeOutput output = new NodeOutput();
            //output.Source = this;
            //Outputs.Add(output);
        }

    }
}
