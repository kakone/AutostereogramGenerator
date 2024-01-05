using System.Windows;

namespace AutostereogramGenerator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class
    /// </summary>
    /// <param name="mainViewModel">main view model</param>
    public MainWindow(IMainViewModel mainViewModel)
    {
        InitializeComponent();

        DataContext = mainViewModel;
    }
}