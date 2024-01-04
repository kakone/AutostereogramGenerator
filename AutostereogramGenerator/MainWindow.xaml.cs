using System.Windows;

namespace AutostereogramGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IMainViewModel mainViewModel)
        {
            InitializeComponent();

            DataContext = mainViewModel;
        }
    }
}