using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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
        private CancellationTokenSource TokenSource;

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

        private string GenerateShowMapTable()
        {
            var tableContent = new StringBuilder();
            var count = 0;
            var encodeMap = EncodeProvider.GetEncodeMap();

            foreach (KeyValuePair<char, char> pair in encodeMap)
            {
                if (tableContent.Length > 0)
                {
                    // if we have contents, append separators.
                    if (count % 10 == 0)
                    {
                        tableContent.Append(",\r\n");
                    }
                    else
                    {
                        tableContent.Append(",\t");
                    }
                }

                tableContent.AppendFormat("{0}={1}", pair.Key, pair.Value);
                count++;
            }

            tableContent.Append('.');       // ending the table with dot
            return tableContent.ToString();
        }

        private void ShowHideMapTable()
        {
            if (ShowMapTable)
            {
                ShowButton.Content = "Hide Table";
                MapTable.Text = GenerateShowMapTable();
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
                UpdateOutputAsync(InputField.Text, string.Empty);
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
