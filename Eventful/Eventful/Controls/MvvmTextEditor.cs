using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Eventful.Controls
{
    public class MvvmTextEditor : TextEditor, INotifyPropertyChanged
    {
        public static DependencyProperty CaretOffsetProperty =
            DependencyProperty.Register("CaretOffset", typeof(int), typeof(MvvmTextEditor),
            // binding changed callback: set value of underlying property
            new PropertyMetadata((obj, args) =>
            {
                MvvmTextEditor target = (MvvmTextEditor)obj;
                target.CaretOffset = (int)args.NewValue;
            })
        );

        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MvvmTextEditor),
            // binding changed callback: set value of underlying property
            new PropertyMetadata((obj, args) =>
            {
                MvvmTextEditor target = (MvvmTextEditor) obj;
                target.Document.Text = (string) args.NewValue;
            })
        );

        public new int CaretOffset
        {
            get { return base.CaretOffset; }
            set { base.CaretOffset = value; }
        }

        public new string Text
        {
            get { return base.Document.Text; }
            set { base.Document.Text = value; }
        }

        public int Length { get { return base.Document.Text.Length; } }

        protected override void OnTextChanged(EventArgs e)
        {
            RaisePropertyChanged("Length");
            RaisePropertyChanged("Text");
            RaisePropertyChanged("CaretOffset");
            base.OnTextChanged(e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
