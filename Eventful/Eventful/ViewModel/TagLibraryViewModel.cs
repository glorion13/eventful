using Eventful.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

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
            Tags = new ObservableCollection<Tag>();
            TagFilter = "";
            TagsViewSource = new CollectionViewSource();
            TagsViewSource.Source = Tags;
            TagsViewSource.View.SortDescriptions.Add(new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));
        }
        private void InitialiseInDesignMode()
        {
            Tags.Add(new Tag("Kingslayer"));
            Tags.Add(new Tag("Nautical-looking"));
        }
        private void InitialiseInRealMode()
        {
            Tags.Add(new Tag("Kingslayer"));
            Tags.Add(new Tag("Nautical-looking"));
            Tags.Add(new Tag("Kingslayer"));
        }

        private ObservableCollection<Tag> tags;
        public ObservableCollection<Tag> Tags
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

        private Tag selectedTag;
        public Tag SelectedTag
        {
            get
            {
                return selectedTag;
            }
            set
            {
                Set(() => SelectedTag, ref selectedTag, value);
            }
        }

        private bool isAddTagButtonEnabled = true;
        public bool IsAddTagButtonEnabled
        {
            get
            {
                return isAddTagButtonEnabled;
            }
            set
            {
                Set(() => IsAddTagButtonEnabled, ref isAddTagButtonEnabled, value);
            }
        }

        private bool isRemoveTagButtonEnabled = false;
        public bool IsRemoveTagButtonEnabled
        {
            get
            {
                return isRemoveTagButtonEnabled;
            }
            set
            {
                Set(() => IsRemoveTagButtonEnabled, ref isRemoveTagButtonEnabled, value);
            }
        }

        private string tagFilter;
        public string TagFilter
        {
            get
            {
                return tagFilter;
            }
            set
            {
                Set(() => TagFilter, ref tagFilter, value);
                if (TagsViewSource != null)
                    TagsViewSource.View.Filter = new Predicate<object>(TagTitleContains);
            }
        }

        private bool TagTitleContains(object obj)
        {
            Tag tag = obj as Tag;
            if (tag == null) return false;
            return (CultureInfo.CurrentCulture.CompareInfo.IndexOf(tag.Title, TagFilter, CompareOptions.IgnoreCase) >= 0);
        }

        public CollectionViewSource TagsViewSource { get; set; }

    }
}
