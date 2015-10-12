using System;
using MahApps.Metro.Controls;
using GalaSoft.MvvmLight.Messaging;
using Eventful.ViewModel;

namespace Eventful.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<EditEventViewModel>(this, vm => OpenNewEventWindow(vm));
            Messenger.Default.Register<TagLibraryViewModel>(this, vm => OpenTagLibraryWindow(vm));
            Messenger.Default.Register<VariableLibraryViewModel>(this, vm => OpenVariableLibraryWindow(vm));
        }

        private void OpenNewEventWindow(EditEventViewModel vm)
        {
            MetroWindow newWindow = new EditEventWindow();
            newWindow.DataContext = vm;
            newWindow.Show();
        }

        MetroWindow tagLibraryWindow;
        private void OpenTagLibraryWindow(TagLibraryViewModel vm)
        {
            if (tagLibraryWindow == null)
            {
                tagLibraryWindow = new TagLibraryWindow();
                tagLibraryWindow.Closed += TagLibraryWindow_Closed;
                tagLibraryWindow.DataContext = vm;
                tagLibraryWindow.Show();
            }
            else
                tagLibraryWindow.Activate();
        }
        private void TagLibraryWindow_Closed(object sender, EventArgs e)
        {
            tagLibraryWindow = null;
        }

        MetroWindow variableLibraryWindow;
        private void OpenVariableLibraryWindow(VariableLibraryViewModel vm)
        {
            if (variableLibraryWindow == null)
            {
                variableLibraryWindow = new VariableLibraryWindow();
                variableLibraryWindow.Closed += VariableLibraryWindow_Closed;
                variableLibraryWindow.DataContext = vm;
                variableLibraryWindow.Show();
            }
            else
                variableLibraryWindow.Activate();
        }
        private void VariableLibraryWindow_Closed(object sender, EventArgs e)
        {
            variableLibraryWindow = null;
        }
    }
}
