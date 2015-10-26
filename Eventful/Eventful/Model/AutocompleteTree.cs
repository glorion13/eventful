using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventful.Model
{
    public class AutocompleteTree : ViewModelBase
    {
        public AutocompleteTree()
        {
        }

        public AutocompleteTree(AutocompleteData data)
        {
            ParentNode = data;
        }

        public AutocompleteData parentNode;
        public AutocompleteData ParentNode
        {
            get
            {
                return parentNode;
            }
            set
            {
                Set(() => ParentNode, ref parentNode, value);
            }
        }

        private ObservableCollection<AutocompleteTree> childrenNodes = new ObservableCollection<AutocompleteTree>();
        public ObservableCollection<AutocompleteTree> ChildrenNodes
        {
            get
            {
                return childrenNodes;
            }
            set
            {
                Set(() => ChildrenNodes, ref childrenNodes, value);
            }
        }

    }
}
