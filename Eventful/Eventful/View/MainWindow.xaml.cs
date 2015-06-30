using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using GalaSoft.MvvmLight.Messaging;
using Eventful.ViewModel;
using System.Xml;
using System.IO;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;

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

            SetupTextEditorSyntaxHighlight();
            SetupTextEditorAutocomplete();
        }

        private void SetupTextEditorSyntaxHighlight()
        {
            byte[] syntax = Eventful.Properties.Resources.ESL;
            Stream stream = new MemoryStream(syntax);
            using (XmlTextReader reader = new XmlTextReader(stream))
            {
                mvvmTextEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }

        private void SetupTextEditorAutocomplete()
        {
            mvvmTextEditor.TextArea.TextEntering += mvvmTextEditorTextAreaTextEntering;
            mvvmTextEditor.TextArea.TextEntered += mvvmTextEditorTextAreaTextEntered;
        }

        CompletionWindow completionWindow;
        private void mvvmTextEditorTextAreaTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ":")
            {
                // Open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(mvvmTextEditor.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                data.Add(new MyCompletionData("Item1"));
                data.Add(new MyCompletionData("Item2"));
                data.Add(new MyCompletionData("Item3"));
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }
        }

        private void mvvmTextEditorTextAreaTextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        private void OpenNewEventWindow(EditEventViewModel vm)
        {
            MetroWindow newWindow = new EditEventWindow();
            newWindow.DataContext = vm;
            newWindow.Show();
        }

        private void OpenTagLibraryWindow(TagLibraryViewModel vm)
        {
            MetroWindow newWindow = new TagLibraryWindow();
            newWindow.DataContext = vm;
            newWindow.Show();
        }
    }

    /// Implements AvalonEdit ICompletionData interface to provide the entries in the
    /// completion drop down.
    public class MyCompletionData : ICompletionData
    {
        public MyCompletionData(string text)
        {
            this.Text = text;
        }

        public System.Windows.Media.ImageSource Image
        {
            get { return null; }
        }

        public double Priority { get { return 0; } }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return this.Text; }
        }

        public object Description
        {
            get { return "Description for " + this.Text; }
        }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}
