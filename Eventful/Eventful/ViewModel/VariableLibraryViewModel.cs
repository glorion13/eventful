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
            MessengerInstance.Register<ObservableCollection<Variable>>(this, vars => Variables = vars);
        }
        private void InitialiseInDesignMode()
        {
            Variables.Add(new Variable("Kingslayer", "Otan skotwneis vasilia."));
            Variables.Add(new Variable("Nautical-looking", "Otan eisai mauros."));
            Variables.Add(new Variable("fasoli", "posa fasolia exeis?"));
        }
        private void InitialiseInAllModes()
        {
            VariableFilter = "";
            VariableViewSource = new CollectionViewSource();
            Variables = new ObservableCollection<Variable>();
            VariableViewSource.Source = Variables;
            VariableViewSource.View.SortDescriptions.Add(new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));
        }

        private ObservableCollection<Variable> variables;
        public ObservableCollection<Variable> Variables
        {
            get
            {
                return variables;
            }
            set
            {
                Set(() => Variables, ref variables, value);
                if (Variables != null)
                {
                    VariableViewSource.Source = Variables;
                    VariableViewSource.View.SortDescriptions.Add(new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));
                }
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

        private Variable selectedVariable;
        public Variable SelectedVariable
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
            Variable variable = obj as Variable;
            if (variable == null) return false;
            return (CultureInfo.CurrentCulture.CompareInfo.IndexOf(variable.Title, VariableFilter, CompareOptions.IgnoreCase) >= 0);
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
            bool dialogResult = await MessageWindowsViewModel.ShowOkCancelMessage("Confirm Variable Deletion", String.Concat("Do you want to delete the variable \"", SelectedVariable.Title, "\"?"));
            if (dialogResult)
                RemoveVariable(SelectedVariable);
        }
        private void RemoveVariable(Variable variable)
        {
            if (variable != null)
            {
                Variables.Remove(variable);
                MessengerInstance.Send<ObservableCollection<Variable>>(Variables);
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
            if (Variables != null)
            {
                string dialogResult = await MessageWindowsViewModel.ShowOkCancelInput("Create New Variable", text);
                if (dialogResult == null)
                {
                }
                else if (Variables.Any(v => String.Equals(v.Title, dialogResult, StringComparison.OrdinalIgnoreCase)))
                    AddNewVariable(String.Concat("A variable with the title \"", dialogResult, "\" already exists. Please enter a different title."));
                else if (dialogResult == "")
                    AddNewVariable("A variable title cannot be empty.");
                else
                {
                    Variable tempVariable = new Variable(dialogResult);
                    CreateVariable(tempVariable);
                }
            }
        }
        private async void CreateVariable(Variable variable)
        {
            Variables.Add(variable);
            SelectedVariable = variable;
            /*bool success = DataStorage.SaveDeckToDisk(deck);
            if (success)
            {
                Decks.Add(deck);
                SelectedDeck = deck;
            }
            else
            {
                await MessageWindowsViewModel.ShowOkMessage("Couldn't Save Deck", "The deck was not saved successfully. Try again later and ensure the save folder is accessible.");
            }
            */
        }

    }

}
