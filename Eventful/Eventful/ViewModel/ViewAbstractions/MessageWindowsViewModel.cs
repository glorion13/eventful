using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace Eventful.ViewModel
{
    public static class MessageWindowsViewModel
    {
        public static async Task ShowOkMessage(string title, string body)
        {
            MetroWindow metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
            await metroWindow.ShowMessageAsync(title, body, MessageDialogStyle.Affirmative);
        }
        public static async Task<bool> ShowOkCancelMessage(string title, string body)
        {
            MetroWindow metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
            MessageDialogResult dialogResult = await metroWindow.ShowMessageAsync(title, body, MessageDialogStyle.AffirmativeAndNegative);
            return dialogResult == MessageDialogResult.Affirmative;
        }
        public static async Task<string> ShowOkCancelInput(string title, string body)
        {
            MetroWindow metroWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
            string dialogResult = await metroWindow.ShowInputAsync(title, body);
            return dialogResult;
        }
    }
}
