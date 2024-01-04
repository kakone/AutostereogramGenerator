using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace AutostereogramGenerator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class
    /// </summary>
    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    private IServiceProvider ServiceProvider { get; }

    private void ApplicationStartup(object sender, StartupEventArgs e)
    {
        ServiceProvider.GetRequiredService<MainWindow>().Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<MainWindow>();
        services.AddTransient<IMainViewModel, MainViewModel>();
    }
}
