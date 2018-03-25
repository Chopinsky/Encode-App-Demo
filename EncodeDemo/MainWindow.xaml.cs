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
            LastInput = InputField.Text;  // if we decide to use default text, update last input now
        }

        #region Private members

        private string LastInput = string.Empty;
        private IEncodeProvider EncodeProvider = null;

        private Task<string> EncodeTask = null;
        private CancellationTokenSource TokenSource = null;

        #endregion

        #region Override

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            TryCancelEncodeTask();

            if (TokenSource != null)
            {
                TokenSource.Dispose();
            }
        }

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

        private void TryCancelEncodeTask()
        {
            if (EncodeTask != null && EncodeTask.Status == TaskStatus.RanToCompletion)
            {
                TokenSource.Cancel();
            }
        }

        private async void UpdateOutputAsync(string current, string encoded)
        {
            RaiseShield();

            if (TokenSource == null)
            {
                // Lazy initialization
                TokenSource = new CancellationTokenSource();
            }

            // Cancel any currently run task and run the new task
            TryCancelEncodeTask();
            EncodeTask = Task.Run(() =>
            {
                return EncodeService.Encode(current, LastInput, encoded, EncodeProvider);
            }, TokenSource.Token);

            var text = await EncodeTask;
            if (EncodeTask != null && EncodeTask.Status == TaskStatus.RanToCompletion)
            {
                OutputField.Text = text;
                EncodeTask = null;
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
            
            // Update last
            LastInput = InputField.Text;

            if (string.IsNullOrEmpty(InputField.Text))
            {
                OutputField.Text = string.Empty;
            }
            else if (InputField.Text.Length < EncodeProvider.GetSyncEncodeCharLimit())
            {
                OutputField.Text = EncodeService.Encode(InputField.Text, LastInput, OutputField.Text, EncodeProvider);
            }
            else
            {
                UpdateOutputAsync(InputField.Text, OutputField.Text);
            }
        }

        private void OutputField_TextChanged(object sender, TextChangedEventArgs e)
        {
            int line = InputField.GetLineIndexFromCharacterIndex(InputField.CaretIndex);

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
            ShowButton.Content = "Show Translation Table";
        }

        private void RegenerateButton_Click(object sender, RoutedEventArgs e)
        {
            EncodeProvider.RegenerateEncodeTable();
            UpdateOutputAsync(InputField.Text, string.Empty);
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
