using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EncodeDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeEncodeProvider();

            LastInput = InputField.Text;   // if we have default text, update last input now
            ShowMapTable = true;           // if to default to false, update here
            
            ShowHideMapTable();            // this needs to happen after setup the ShowMapTable property
        }

        #region Private members

        private string LastInput;
        private bool ShowMapTable;

        private IEncodeProvider EncodeProvider;
        private Task<string> EncodeTask;

        #endregion

        #region Override

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            InputField.Focus();
        }

        #endregion

        #region Helper methods

        private void InitializeEncodeProvider()
        {
            EncodeProvider = new CyrillicEncodeProvider();
        }

        private async void UpdateOutputAsync(string current)
        {
            RaiseShield();

            EncodeTask = Task.Run(() =>
            {
                return EncodeService.Encode(current, EncodeProvider);
            });

            var text = await EncodeTask;
            if (EncodeTask != null && EncodeTask.IsCompleted)
            {
                OutputField.Text = text;
                EncodeTask = null;
            }
        }

        private void ShowHideMapTable()
        {
            if (ShowMapTable)
            {
                ShowButton.Content = "Hide Table";
                MapTable.Text = EncodeService.GenerateShowMapTable(EncodeProvider);
            }
            else
            {
                ShowButton.Content = "Show Table";
                MapTable.Text = string.Empty;
            }
        }

        #endregion

        #region Event handlers

        private void InputField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EncodeProvider == null)
            {
                // This shouldn't happen, but just be cautious
                InitializeEncodeProvider();
            }

            if (string.IsNullOrEmpty(InputField.Text))
            {
                OutputField.Text = string.Empty;
                return;
            }

            UpdateOutputAsync(InputField.Text);            
        }

        private void OutputField_TextChanged(object sender, TextChangedEventArgs e)
        {
            var caretIndex = InputField.CaretIndex;
            if (caretIndex > OutputField.Text.Length)
            {
                // we're not in-synch with the input field yet, wait for the next mapping task to finish
                // and then update the scroll bar position.
                return;
            }

            // After the last update, the caret index shall point to the same position as we do 1-to-1 character
            // mapping here.
            int line = OutputField.GetLineIndexFromCharacterIndex(caretIndex);

            // As OutputField can be updated in async, InputField's line count could be larger than the
            // OutputField line count, and since the update can be behind, we only update the scroll position
            // when the line has been updated in the OutputField
            if (OutputField.LineCount > line)
            {
                try
                {
                    OutputField.Focus();
                    OutputField.ScrollToLine(line);

                    InputField.Focus();
                }
                catch (ArgumentOutOfRangeException err)
                {
                    throw err;
                }
            }

            LowerShield();
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            ShowMapTable = !ShowMapTable;
            ShowHideMapTable();
        }

        private async void RegenerateButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                EncodeProvider.RegenerateEncodeTable();
            });

            ShowHideMapTable();

            if (string.IsNullOrEmpty(InputField.Text))
            {
                OutputField.Text = string.Empty;
            }
            else
            {
                UpdateOutputAsync(InputField.Text);
            }         
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Output field will be reset by the event
            InputField.Clear();
        }

        #endregion

        #region Shield Updates

        private void RaiseShield()
        {
            if (Shield.Visibility != Visibility.Visible)
            {
                Shield.Visibility = Visibility.Visible;
            }
        }

        private void LowerShield()
        {
            if (Shield.Visibility != Visibility.Hidden)
            {
                Shield.Visibility = Visibility.Hidden;
            }
        }

        #endregion
    }
}
