using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls.Primitives;
using System.Xml.Serialization;

namespace Eventful.ViewModel
{
    [Serializable]
    public class Screen : ViewModelBase
    {
        public Screen()
        {
            MessengerInstance.Register<Option>(this, option => OptionDraggedOverScreen(option));
            Width = 150;
            Height = 40;
        }

        public Screen(Screen screen)
        {
            Title = screen.Title;
            Text = screen.Text;
            foreach (Option option in screen.Options)
                AddOption(option, true);
            Width = 150;
            Height = 40;
            X = screen.X;
            Y = screen.Y;
            Update();
            MessengerInstance.Register<Option>(this, option => OptionDraggedOverScreen(option));
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

        private Guid id = Guid.NewGuid();
        public Guid Id
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

        private string text = "";
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

        private double x = 0;
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                Set(() => X, ref x, Math.Max(0, value));
                Update();
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
                Set(() => Y, ref y, Math.Max(0, value));
                Update();
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

        [XmlIgnore]
        private double width = 150;
        [XmlIgnore]
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

        [XmlIgnore]
        private double height = 40;
        [XmlIgnore]
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

        [XmlIgnore]
        private bool isSelected = false;
        [XmlIgnore]
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                Set(() => IsSelected, ref isSelected, value);
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
        private void OptionDraggedOverScreen(Option option)
        {
            if (Options.Contains(option)) return;
            else if ((option.Hotspot.InputX > X) && (option.Hotspot.InputX < X + Width) && (option.Hotspot.InputY > Y) && (option.Hotspot.InputY < Y + Height))
            {
                option.TargetId = Id;
                option.UpdateTargetFromId();
            }
        }

        private void UpdateInputPositions()
        {
            InputX = X + (Width / 2);
            InputY = Y;
        }

        private void UpdateOutputPositions()
        {
            foreach (Option option in Options)
                option.Update();
        }

        public void Update()
        {
            UpdateInputPositions();
            UpdateOutputPositions();
            if (ParentEvent != null)
                ParentEvent.IsChanged = true;
        }

        public int GetOptionIndex(Option output)
        {
            return Options.IndexOf(output);
        }

        public void AddOption()
        {
            Option option = new Option();
            option.Index = Options.Count + 1;
            option.Source = this;
            option.Target = null;
            Options.Add(option);
            if (ParentEvent != null)
                ParentEvent.IsChanged = true;
        }
        public void AddOption(Option sourceOption)
        {
            Option option = new Option();
            option.Index = sourceOption.Index;
            option.Text = sourceOption.Text;
            option.Source = this;
            option.Target = null;
            Options.Add(option);
            if (ParentEvent != null)
                ParentEvent.IsChanged = true;
        }
        public void AddOption(Option sourceOption, bool keepConnections)
        {
            Option option = new Option();
            option.Index = sourceOption.Index;
            option.Text = sourceOption.Text;
            option.Source = this;
            option.Target = sourceOption.Target;
            Options.Add(option);
            if (ParentEvent != null)
                ParentEvent.IsChanged = true;
        }

        public void RemoveOption(Option option)
        {
            foreach (Option opt in Options)
                if (opt.Index > option.Index)
                    opt.Index--;
            int index = option.Index;
            Options.Remove(option);
        }

        private RelayCommand addOptionCommand;
        public RelayCommand AddOptionCommand
        {
            get
            {
                return addOptionCommand ?? (addOptionCommand = new RelayCommand(ExecuteAddOptionCommand));
            }
        }
        private void ExecuteAddOptionCommand()
        {
            AddOption();
            //InitialiseConnections();
            //SelectedOption = SelectedScreen.Options.Last();
        }

        private RelayCommand<Option> removeOptionCommand;
        public RelayCommand<Option> RemoveOptionCommand
        {
            get
            {
                return removeOptionCommand ?? (removeOptionCommand = new RelayCommand<Option>(ExecuteRemoveOptionCommand));
            }
        }
        private void ExecuteRemoveOptionCommand(Option option)
        {
            RemoveOption(option);

            //int index = option.Index;
            /*if (index >= 2)
                SelectedOption = SelectedScreen.Options[index - 2];
            else if (SelectedScreen.Options.Count > 0)
                SelectedOption = SelectedScreen.Options[0];
            if (SelectedScreen.ParentEvent != null)
                SelectedScreen.ParentEvent.IsChanged = true;
            InitialiseConnections();*/
        }

        private RelayCommand<Option> moveOptionDownCommand;
        public RelayCommand<Option> MoveOptionDownCommand
        {
            get
            {
                return moveOptionDownCommand ?? (moveOptionDownCommand = new RelayCommand<Option>(ExecuteMoveOptionDownCommand));
            }
        }
        private void ExecuteMoveOptionDownCommand(Option option)
        {
            if (option.Index == Options.Count) return;
            Option optionAbove = Options.FirstOrDefault(opt => opt.Index == option.Index + 1);
            optionAbove.Index = option.Index;
            option.Index++;
            Options.Move(optionAbove.Index - 1, option.Index - 1);
            Update();
            //InitialiseConnections();
        }

        private RelayCommand<Option> moveOptionUpCommand;
        public RelayCommand<Option> MoveOptionUpCommand
        {
            get
            {
                return moveOptionUpCommand ?? (moveOptionUpCommand = new RelayCommand<Option>(ExecuteMoveOptionUpCommand));
            }
        }
        private void ExecuteMoveOptionUpCommand(Option option)
        {
            if (option.Index == 1) return;
            Option optionAbove = Options.FirstOrDefault(opt => opt.Index == option.Index - 1);
            optionAbove.Index = option.Index;
            option.Index--;
            Options.Move(optionAbove.Index - 1, option.Index - 1);
            Update();
            //InitialiseConnections();
        }

    }
}
