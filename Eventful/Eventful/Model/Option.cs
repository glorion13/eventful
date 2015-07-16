using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventful.Model
{
    public class Option : ViewModelBase
    {
        public Option()
        {
        }

        private Event resultingEvent;
        public Event ResultingEvent
        {
            get
            {
                return resultingEvent;
            }
            set
            {
                Set(() => ResultingEvent, ref resultingEvent, value);
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

        private ObservableCollection<Result> results;
        public ObservableCollection<Result> Results
        {
            get
            {
                return results;
            }
            set
            {
                Set(() => Results, ref results, value);
            }
        }

    }
}
