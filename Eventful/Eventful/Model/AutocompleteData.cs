using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;

namespace Eventful.Model
{
    /// Implements AvalonEdit ICompletionData interface to provide the entries in the
    /// completion drop down.
    public class AutocompleteData : ICompletionData
    {
        public AutocompleteData()
        {
        }
        public AutocompleteData(string text)
        {
            this.Text = text;
        }
        public AutocompleteData(string text, string description)
        {
            this.Text = text;
            this.DescriptionText = text;
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

        private string descriptionText;
        public string DescriptionText
        {
            get
            {
                return descriptionText;
            }
            set
            {
                descriptionText = value;
            }
        }
        public object Description
        {
            get { return DescriptionText; }
        }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
