﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace Toxy.Views
{
    /// <summary>
    /// Interaction logic for SwitchProfileDialog.xaml
    /// </summary>
    public partial class SwitchProfileDialog : BaseMetroDialog
    {
        internal SwitchProfileDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        internal SwitchProfileDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            Title = "Switch Profile";
            InitializeComponent();
        }

        public Task<ComboBoxItem> WaitForButtonPressAsync()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Focus();
                PART_ProfileComboBox.Focus();
            }));

            var tcs = new TaskCompletionSource<ComboBoxItem>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            RoutedEventHandler importHandler = null; 

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = () =>
            {
                this.KeyDown -= escapeKeyHandler;

                PART_NegativeButton.Click -= negativeHandler;
                PART_AffirmativeButton.Click -= affirmativeHandler;

                PART_NegativeButton.KeyDown -= negativeKeyHandler;
                PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;
            };

            escapeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(null);
                }
            };

            negativeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(null);
                }
            };

            affirmativeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(Input);
                }
            };

            negativeHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(null);

                e.Handled = true;
            };

            affirmativeHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(Input);

                e.Handled = true;
            };

            importHandler = (sender, e) =>
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = Environment.CurrentDirectory;
                dialog.Multiselect = false;
                if (dialog.ShowDialog() != true)
                    return;

                string fileName = dialog.FileName;
                if (string.IsNullOrEmpty(fileName))
                    return;

                //show the 'give this profile a name' dialog

                cleanUpHandlers();

                tcs.TrySetResult(Input);

                e.Handled = true;
            };

            PART_NegativeButton.KeyDown += negativeKeyHandler;
            PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;

            this.KeyDown += escapeKeyHandler;

            PART_NegativeButton.Click += negativeHandler;
            PART_AffirmativeButton.Click += affirmativeHandler;
            PART_ImportButton.Click += importHandler;

            return tcs.Task;
        }

        private void Dialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.AffirmativeButtonText = this.DialogSettings.AffirmativeButtonText;
            this.NegativeButtonText = this.DialogSettings.NegativeButtonText;

            switch (this.DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    this.PART_NegativeButton.Style = this.FindResource("HighlightedSquareButtonStyle") as Style;
                    PART_ProfileComboBox.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    break;
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(InputDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty InputProperty = DependencyProperty.Register("Input", typeof(ComboBoxItem), typeof(InputDialog), new PropertyMetadata(default(ComboBoxItem)));
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register("AffirmativeButtonText", typeof(string), typeof(InputDialog), new PropertyMetadata("OK"));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(InputDialog), new PropertyMetadata("Cancel"));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public ComboBoxItem Input
        {
            get { return (ComboBoxItem)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public string AffirmativeButtonText
        {
            get { return (string)GetValue(AffirmativeButtonTextProperty); }
            set { SetValue(AffirmativeButtonTextProperty, value); }
        }

        public string NegativeButtonText
        {
            get { return (string)GetValue(NegativeButtonTextProperty); }
            set { SetValue(NegativeButtonTextProperty, value); }
        }
    }
}
