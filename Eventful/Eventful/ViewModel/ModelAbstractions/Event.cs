using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Eventful.ViewModel
{
    [Serializable]
    public class Event : ViewModelBase
    {
        public Event()
        {
        }
        public Event(string title)
        {
            Title = title;
        }
        public Event(Event ev)
        {
            Title = ev.Title;
            Author = ev.Author;
            foreach (Screen screen in ev.Screens)
                AddScreen(new Screen(screen), true);
        }

        private ObservableCollection<Screen> screens = new ObservableCollection<Screen>();
        public ObservableCollection<Screen> Screens
        {
            get
            {
                return screens;
            }
            set
            {
                Set(() => Screens, ref screens, value);
            }
        }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                Set(() => Title, ref title, value);
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

        private string author = "anonymous";
        public string Author
        {
            get
            {
                return author;
            }
            set
            {
                Set(() => Author, ref author, value);
            }
        }

        private DateTime date = DateTime.Now;
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                Set(() => Date, ref date, value);
            }
        }

        private bool isChanged = true;
        public bool IsChanged
        {
            get
            {
                return isChanged;
            }
            set
            {
                Set(() => IsChanged, ref isChanged, value);
            }
        }

        public void AddScreen(string screenName)
        {
            Screen screen = new Screen();
            screen.ParentEvent = this;
            screen.Title = screenName;
            screen.X = 10;
            screen.Y = 10;
            Screens.Add(screen);
        }
        public void AddScreen(Screen screen)
        {
            screen.ParentEvent = this;
            screen.X = 10;
            screen.Y = 10;
            Screens.Add(screen);
        }
        public void AddScreen(Screen screen, bool keepScreenPosition)
        {
            screen.ParentEvent = this;
            if (!keepScreenPosition)
            {
                screen.X = 10;
                screen.Y = 10;
            }
            Screens.Add(screen);
        }

        public void RemoveScreen(Screen screen)
        {
            Screens.Remove(screen);
        }

        private RelayCommand addScreenCommand;
        public RelayCommand AddScreenCommand
        {
            get
            {
                return addScreenCommand ?? (addScreenCommand = new RelayCommand(ExecuteAddScreenCommand));
            }
        }
        private async void ExecuteAddScreenCommand()
        {
            string dialogResult = await MessageWindowsViewModel.ShowOkCancelInput("Create New Screen", "What is the title of the new screen? You can always change it later.");
            if (dialogResult != null)
            {
                AddScreen(dialogResult);
                //ExecuteSelectScreenCommand(SelectedEvent.Screens.Last());
                //InitialiseConnections();
            }
        }

        private RelayCommand<Screen> removeScreenCommand;
        public RelayCommand<Screen> RemoveScreenCommand
        {
            get
            {
                return removeScreenCommand ?? (removeScreenCommand = new RelayCommand<Screen>(ExecuteRemoveScreenCommand));
            }
        }
        private async void ExecuteRemoveScreenCommand(Screen screen)
        {
            if (screen == null) return;
            bool dialogResult = await MessageWindowsViewModel.ShowOkCancelMessage("Confirm Screen Deletion", $"Do you want to delete the screen {screen.Title}?");
            if (dialogResult)
            {
                RemoveScreen(screen);
                //SelectedScreen = SelectedScreen == screen ? null : SelectedScreen;
                //InitialiseConnections();
            }
        }
    }
}
