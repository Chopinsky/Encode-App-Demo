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

        }

        private void InputField_TextChanged(object sender, TextChangedEventArgs e)
        {
            OutputField.Text = InputField.Text;
        }
    }
}
