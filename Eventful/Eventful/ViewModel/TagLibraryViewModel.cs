using Eventful.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventful.ViewModel
{
    public class TagLibraryViewModel : ViewModelBase
    {
        public TagLibraryViewModel()
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
            Tags = new HashSet<Tag>();
        }
        private void InitialiseInDesignMode()
        {
        }
        private void InitialiseInRealMode()
        {
        }

        private HashSet<Tag> tags;
        public HashSet<Tag> Tags
        {
            get
            {
                return tags;
            }
            set
            {
                Set(() => Tags, ref tags, value);
            }
        }

    }
}
