using Eventful.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
    public class VariableLibraryViewModel : ViewModelBase
    {
        public VariableLibraryViewModel()
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

        private void InitialiseInRealMode()
        {
            MessengerInstance.Register<ObservableCollection<string>>(this, vars => Variables = vars);
            VariableFilter = "";
            VariableViewSource = new CollectionViewSource();
            VariableViewSource.Source = Variables;
            VariableViewSource.View.SortDescriptions.Add(new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));
        }
        private void InitialiseInDesignMode()
        {
            Variables.Add("Kingslayer");
            Variables.Add("Nautical-looking");
            Variables.Add("fasoli");
        }
        private void InitialiseInAllModes()
        {
            Variables = new ObservableCollection<string>();
        }

        private ObservableCollection<string> variables;
        public ObservableCollection<string> Variables
        {
            get
            {
                return variables;
            }
            set
            {
                Set(() => Variables, ref variables, value);
            }
        }

        private bool isAddVariableButtonEnabled = true;
        public bool IsAddVariableButtonEnabled
        {
            get
            {
                return isAddVariableButtonEnabled;
            }
            set
            {
                Set(() => IsAddVariableButtonEnabled, ref isAddVariableButtonEnabled, value);
            }
        }

        private bool isRemoveVariableButtonEnabled = false;
        public bool IsRemoveVariableButtonEnabled
        {
            get
            {
                return isRemoveVariableButtonEnabled;
            }
            set
            {
                Set(() => IsRemoveVariableButtonEnabled, ref isRemoveVariableButtonEnabled, value);
            }
        }

        private string selectedVariable;
        public string SelectedVariable
        {
            get
            {
                return selectedVariable;
            }
            set
            {
                Set(() => SelectedVariable, ref selectedVariable, value);
                IsRemoveVariableButtonEnabled = SelectedVariable == null ? false : true;
            }
        }

        private string variableFilter;
        public string VariableFilter
        {
            get
            {
                return variableFilter;
            }
            set
            {
                Set(() => VariableFilter, ref variableFilter, value);
                if (VariableViewSource != null)
                    VariableViewSource.View.Filter = new Predicate<object>(VariableNameContains);
            }
        }

        private bool VariableNameContains(object obj)
        {
            string variable = obj as string;
            if (variable == null) return false;
            return (CultureInfo.CurrentCulture.CompareInfo.IndexOf(variable, VariableFilter, CompareOptions.IgnoreCase) >= 0);
        }

        public CollectionViewSource VariableViewSource { get; set; }

        private RelayCommand removeVariableCommand;
        public RelayCommand RemoveVariableCommand
        {
            get
            {
                return removeVariableCommand ?? (removeVariableCommand = new RelayCommand(ExecuteRemoveVariableCommand));
            }
        }
        private async void ExecuteRemoveVariableCommand()
        {
            bool dialogResult = await MessageWindowsViewModel.ShowOkCancelMessage("Confirm Variable Deletion", String.Concat("Do you want to delete the variable \"", SelectedVariable, "\"?"));
            if (dialogResult)
                RemoveVariable(SelectedVariable);
        }
        private void RemoveVariable(string variable)
        {
            if (variable != null)
            {
                Variables.Remove(variable);
                MessengerInstance.Send<ObservableCollection<string>>(Variables);
            }
        }

        private RelayCommand addVariableCommand;
        public RelayCommand AddVariableCommand
        {
            get
            {
                return addVariableCommand ?? (addVariableCommand = new RelayCommand(ExecuteAddVariableCommand));
            }
        }
        private void ExecuteAddVariableCommand()
        {
            AddNewVariable("What is the name of the new variable? You can always change it later.");
        }
        private async void AddNewVariable(string text)
        {
            /*if (Decks != null)
            {
                string dialogResult = await ShowOkCancelInput("Create New Deck", text);
                if (dialogResult == null)
                {
                }
                else if (Decks.Any(d => String.Equals(d.Title, dialogResult, StringComparison.OrdinalIgnoreCase)))
                    AddNewVariable(String.Concat("A deck with the title \"", dialogResult, "\" already exists. Please enter a different title."));
                else if (dialogResult == "")
                    AddNewVariable("A deck title cannot be empty.");
                else
                {
                    Deck tempDeck = new Deck(dialogResult);
                    CreateDeck(tempDeck);
                }
            }*/
        }
        /*private async void CreateDeck(Deck deck)
        {
            bool success = DataStorage.SaveDeckToDisk(deck, StorageDirectory);
            if (success)
            {
                Decks.Add(deck);
                SelectedDeck = deck;
            }
            else
            {
                await ShowOkMessage("Couldn't Save Deck", "The deck was not saved successfully. Try again later and ensure the save folder is accessible.");
            }
            }
            */

    }

}
