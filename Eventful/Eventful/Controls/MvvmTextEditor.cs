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
using System.Linq;

namespace Eventful.Controls
{
    public class MvvmTextEditor : TextEditor
    {
        FoldingManager textEditorFoldingManager;
        XmlFoldingStrategy textEditorFoldingStrategy;
        AutocompleteTree lastSelectedAutocompleteTree;

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

        private void PopupCompletionWindow(IList<AutocompleteTree> trees)
        {
            CompletionWindow completionWindow = new CompletionWindow(base.TextArea);
            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            foreach (AutocompleteTree completionOption in trees)
                data.Add(completionOption.ParentNode);
            completionWindow.Show();
            completionWindow.Closed += delegate
            {
                AutocompleteData chosenOption = completionWindow.CompletionList.ListBox.SelectedItem as AutocompleteData;
                AutocompleteTree chosenTree = AutocompleteTrees.FirstOrDefault(tree => tree.ParentNode == chosenOption);
                if (chosenTree != null)
                {
                    lastSelectedAutocompleteTree = chosenTree;
                }
            };
        }

        private void mvvmTextEditorTextAreaTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "<")
            {
                PopupCompletionWindow(AutocompleteTrees);
            }
            if (e.Text == ".")
            {
                string lastWord = GetLast(Text, lastSelectedAutocompleteTree.ParentNode.Text.Length);
                // Need TO DO this
                if (lastSelectedAutocompleteTree.ParentNode.ToString() == lastWord)
                    PopupCompletionWindow(lastSelectedAutocompleteTree.ChildrenNodes);
            }

            UpdateTextEditorFoldings();
        }


        private string GetLast(string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }

        private void mvvmTextEditorTextAreaTextEntering(object sender, TextCompositionEventArgs e)
        {
            /*if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }*/
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
            "AutocompleteTrees", typeof(IList<AutocompleteTree>), typeof(MvvmTextEditor));

        public IList<AutocompleteTree> AutocompleteTrees
        {
            get { return (IList<AutocompleteTree>) GetValue(AutocompleteDataProperty); }
            set { SetValue(AutocompleteDataProperty, value); }
        }

    }
}
