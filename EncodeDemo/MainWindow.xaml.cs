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
        private static string LastInput = string.Empty;
        private static IEncodeProvider EncodeProvider = null;

        private static Task<string> EncodeTask = null;
        private static CancellationTokenSource TokenSource = null;

        private const int SYNC_ENCODE_CHAR_LIMIT = 128;

        public MainWindow()
        {
            InitializeComponent();
            InitializeEncodeProvider();
            LastInput = InputField.Text;  // if we decide to use default text, update last input now
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            TryCancelEncodeTask();
            TokenSource.Dispose();
        }

        private void InitializeEncodeProvider()
        {
            EncodeProvider = new CyrillicEncodeProvider();
            TokenSource = new CancellationTokenSource();
        }

        private void TryCancelEncodeTask()
        {
            if (EncodeTask != null && EncodeTask.Status == TaskStatus.RanToCompletion)
            {
                TokenSource.Cancel();
            }
        }

        #region Event handlers
        private async void InputField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EncodeProvider == null)
            {
                // This shouldn't happen, but just be cautious
                InitializeEncodeProvider();
            }
            
            string current = InputField.Text;
            string encoded = OutputField.Text;

            // Update last
            LastInput = current;

            if (InputField.Text.Length < SYNC_ENCODE_CHAR_LIMIT)
            {
                OutputField.Text = EncodeService.Encode(current, LastInput, encoded, EncodeProvider);
            }
            else
            {
                RaiseShield();
                TryCancelEncodeTask();

                EncodeTask = Task.Run(() =>
                {
                    string output = EncodeService.Encode(current, LastInput, encoded, EncodeProvider);
                    return output;
                }, TokenSource.Token);

                var text = await EncodeTask;
                if (EncodeTask != null && EncodeTask.Status == TaskStatus.RanToCompletion)
                {
                    OutputField.Text = text;
                    EncodeTask = null;
                }

                LowerShield();
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
        }
        #endregion

        #region Shield Updates
        private void RaiseShield()
        {
            Shield.Visibility = Visibility.Visible;
        }

        private void LowerShield()
        {
            Shield.Visibility = Visibility.Hidden;
        }
        #endregion
    }
}
