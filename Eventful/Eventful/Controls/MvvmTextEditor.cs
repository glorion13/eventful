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
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Windows;
using System.ComponentModel;
using Eventful.Model;

namespace Eventful.Controls
{
    public class MvvmTextEditor : TextEditor
    {
        FoldingManager textEditorFoldingManager;
        XmlFoldingStrategy textEditorFoldingStrategy;
        CompletionWindow completionWindow;
        int characterOffset;

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
        }
        public void UpdateTextEditorFoldings()
        {
            textEditorFoldingStrategy.UpdateFoldings(textEditorFoldingManager, base.Document);
        }

        private void mvvmTextEditorTextAreaTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "<")
            {
                completionWindow = new CompletionWindow(base.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                foreach (AutocompleteTree kek in AutocompleteData)
                    data.Add(kek.ParentNode);
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    string kek = completionWindow.CompletionList.ListBox.SelectedItem.ToString();
                    completionWindow = null;
                };
            }
            if (e.Text == ":")
            {
                if (base.Document.TextLength >= 4)
                {
                    string threeCharacterOffset = base.Document.GetText(base.CaretOffset - 4, 3);
                    if (threeCharacterOffset == "tag")
                    {
                        completionWindow = new CompletionWindow(base.TextArea);
                        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                        data.Add(new AutocompleteData("Kingslayer"));
                        data.Add(new AutocompleteData("Nautical-looking"));
                        completionWindow.Show();
                        completionWindow.Closed += delegate
                        {
                            completionWindow = null;
                        };
                    }
                    else if (threeCharacterOffset == "var")
                    {
                        completionWindow = new CompletionWindow(base.TextArea);
                        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                        data.Add(new AutocompleteData("player_name"));
                        data.Add(new AutocompleteData("player_background"));
                        completionWindow.Show();
                        completionWindow.Closed += delegate
                        {
                            completionWindow = null;
                        };
                    }
                }
            }
            UpdateTextEditorFoldings();
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
            "AutocompleteData", typeof(IList<AutocompleteTree>), typeof(MvvmTextEditor));

        public IList<AutocompleteTree> AutocompleteData
        {
            get { return (IList<AutocompleteTree>) GetValue(AutocompleteDataProperty); }
            set { SetValue(AutocompleteDataProperty, value); }
        }

    }
}
