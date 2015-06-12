using Eventful.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventful.ViewModel
{
    public class EditEventViewModel : ViewModelBase
    {
        public EditEventViewModel()
        {
            InitialiseInAllModes();
            if (IsInDesignMode)
            {
                InitialiseInDesignMode();
            }
            else
            {
                InitialiseInRealMode();
            }
        }
        private void InitialiseInAllModes()
        {
        }
        private void InitialiseInDesignMode()
        {
        }
        private void InitialiseInRealMode()
        {
        }
        
        private Event selectedEvent;
        public Event SelectedEvent
        {
            get
            {
                return selectedEvent;
            }
            set
            {
                Set(() => SelectedEvent, ref selectedEvent, value);
            }
        }

        private Deck selectedDeck;
        public Deck SelectedDeck
        {
            get
            {
                return selectedDeck;
            }
            set
            {
                Set(() => SelectedDeck, ref selectedDeck, value);
            }
        }
    }
}
