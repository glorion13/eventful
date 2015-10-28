using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;
using System.Windows.Input;
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Windows;
using Eventful.Model;
using System.Linq;
using System.Collections;

namespace Eventful.Controls
{
    public class MvvmTextEditor : TextEditor
    {
        FoldingManager textEditorFoldingManager;
        XmlFoldingStrategy textEditorFoldingStrategy;
        CompletionWindow completionWindow;

        public MvvmTextEditor()
        {
            TextArea.TextView.LineTransformers.Add(new SpellingErrorColorizer());
            SetupTextEditorForFolding();
            SetupTextEditorAutocomplete();
            //SetupTextEditorSyntaxHighlight();
        }

        private void SetupTextEditorForFolding()
        {
            textEditorFoldingManager = FoldingManager.Install(base.TextArea);
            textEditorFoldingStrategy = new XmlFoldingStrategy();
        }
        private void SetupTextEditorSyntaxHighlight()
        {
            byte[] syntax = Eventful.Properties.Resources.ESL;
            Stream stream = new MemoryStream(syntax);
            using (XmlTextReader reader = new XmlTextReader(stream))
            {
                base.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }
        private void SetupTextEditorAutocomplete()
        {
            base.TextArea.TextEntering += mvvmTextEditorTextAreaTextEntering;
            base.TextArea.TextEntered += mvvmTextEditorTextAreaTextEntered;
            base.TextArea.Caret.PositionChanged += mvvmTextEditorTextAreaCaretPositionChanged;
        }

        private void mvvmTextEditorTextAreaCaretPositionChanged(object sender, EventArgs e)
        {
            if (CaretOffset > 0)
            {
                if (Text[CaretOffset - 1] == '<')
                    PopupCompletionWindow(AutocompleteTrees.Keys);
                if (Text[CaretOffset - 1] == '.')
                {
                    string word = FindXmlPropertyName(base.Text);
                    if (AutocompleteTrees.ContainsKey(word))
                        PopupCompletionWindow(AutocompleteTrees[word] as ICollection);
                }
            }
        }

        public void UpdateTextEditorFoldings()
        {
            textEditorFoldingStrategy.UpdateFoldings(textEditorFoldingManager, base.Document);
        }

        private void PopupCompletionWindow(ICollection list)
        {
            if (list == null) return;
            completionWindow = new CompletionWindow(base.TextArea);
            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            foreach (var completionOption in list)
            {
                if (typeof(Variable) == completionOption.GetType())
                    data.Add(new AutocompleteData(((Variable)completionOption).Title, ((Variable)completionOption).Description));
                if (typeof(Tag) == completionOption.GetType())
                    data.Add(new AutocompleteData(((Tag)completionOption).Title));
                if (typeof(string) == completionOption.GetType())
                    data.Add(new AutocompleteData((string)completionOption));
            }
            completionWindow.Show();
            completionWindow.Closed += delegate
            {
                completionWindow = null;
            };
        }

        private void mvvmTextEditorTextAreaTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ">")
            {
                string word = FindXmlPropertyName(Text);
                if (word.Length > 0)
                {
                    if (word[0] != '/')
                    {
                        word = word.Split(' ')[0];
                        int offset = base.CaretOffset;
                        Text = Text.Insert(offset, "\n");
                        offset++;
                        Text = Text.Insert(offset, "\n</" + word + ">");
                        base.CaretOffset = offset;
                    }
                }
            }
            UpdateTextEditorFoldings();
        }

        private string FindXmlPropertyName(string text)
        {
            string word = "";
            for (int i = base.CaretOffset - 2; i >= 0; i--)
            {
                if (text[i] == '<')
                    break;
                word = text[i] + word;
            }
            return word;
        }

        private void mvvmTextEditorTextAreaTextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        public static readonly DependencyProperty DocumentTextProperty = DependencyProperty.Register(
            "DocumentText", typeof(string), typeof(MvvmTextEditor), new PropertyMetadata("", OnDocumentTextChanged));

        private static void OnDocumentTextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (MvvmTextEditor)sender;
            if (string.Compare(control.DocumentText, e.NewValue.ToString()) != 0)
            {
                //avoid undo stack overflow
                control.DocumentText = e.NewValue.ToString();
            }
        }

        public string DocumentText
        {
            get { return Text; }
            set { Text = value; }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            SetCurrentValue(DocumentTextProperty, Text);
            base.OnTextChanged(e);
        }

        public static DependencyProperty AutocompleteDataProperty = DependencyProperty.Register(
            "AutocompleteTrees", typeof(Hashtable), typeof(MvvmTextEditor));

        public Hashtable AutocompleteTrees
        {
            get { return (Hashtable) GetValue(AutocompleteDataProperty); }
            set { SetValue(AutocompleteDataProperty, value); }
        }

    }
}
