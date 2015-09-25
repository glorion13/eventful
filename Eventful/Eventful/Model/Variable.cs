using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventful.Model
{
    public class Variable : ViewModelBase
    {
        public Variable()
        {
            Title = "";
            Description = "";
        }
        public Variable(string title)
        {
            Title = title;
            Description = "";
        }
        public Variable(string title, string description)
        {
            Title = title;
            Description = description;
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

        private string description;
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                Set(() => Description, ref description, value);
            }
        }
    }
}
