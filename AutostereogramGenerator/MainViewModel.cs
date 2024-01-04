using Autostereogram;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutostereogramGenerator;

public partial class MainViewModel : ObservableObject, IMainViewModel
{
    public MainViewModel()
    {
        SizeChangedTimer = new Timer(RefreshAutostereogramAsync);
        DepthMaps = GetFiles(nameof(DepthMaps), Settings.Default.DepthMapsPath);
        Patterns = GetFiles(nameof(Patterns), Settings.Default.PatternsPath);
    }

    private Timer SizeChangedTimer { get; }

    private int Dpi { get; set; }

    private int Width { get; set; }

    private int Height { get; set; }

    public IEnumerable<File> Patterns { get; }

    public IEnumerable<File> DepthMaps { get; }

    public static string? ApplicationTitle => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>()?.Title;

    [ObservableProperty]
    private string? _depthMap;

    [ObservableProperty]
    private string? _pattern;

    [ObservableProperty]
    private ImageSource? _autostereogram;

    partial void OnDepthMapChanged(string? value)
    {
        RefreshAutostereogramAsync();
    }

    partial void OnPatternChanged(string? value)
    {
        RefreshAutostereogramAsync();
    }

    private static string[] GetFiles(string path)
    {
        try
        {
            return Directory.GetFiles(path);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private static File[] GetFiles(string applicationPath, string userPath)
    {
        return
        [
            .. new[] { string.Empty }.Concat(GetFiles(applicationPath)).Concat(GetFiles(userPath))
                .Select(path => new File(path)).OrderBy(f => f.Name)
        ];
    }

    private static async Task<Image<Rgb24>> LoadImageAsync(string image)
    {
        return await Image.LoadAsync<Rgb24>(image);
    }

    private static async Task<BitmapImage> ToBitmapImageAsync(Image<Rgb24> image)
    {
        var bitmapImage = new BitmapImage
        {
            CacheOption = BitmapCacheOption.OnLoad
        };
        bitmapImage.BeginInit();
        try
        {
            var memoryStream = new MemoryStream();
            await image.SaveAsBmpAsync(memoryStream);
            memoryStream.Position = 0;
            bitmapImage.StreamSource = memoryStream;
        }
        finally
        {
            bitmapImage.EndInit();
        }
        return bitmapImage;
    }

    private async void RefreshAutostereogramAsync(object? state = null)
    {
        SizeChangedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        var depthMap = DepthMap;
        if (string.IsNullOrEmpty(depthMap))
        {
            await Application.Current.Dispatcher.InvokeAsync(() => Autostereogram = null);
            return;
        }
        using var depthMapImage = await LoadImageAsync(depthMap);
        var pattern = Pattern;
        var patternImage = string.IsNullOrEmpty(pattern) ? null : await LoadImageAsync(pattern);
        try
        {
            using var autostereogram = new AutoStereogramGenerator(depthMapImage, patternImage, Width, Height, Dpi).Create();
            await Application.Current.Dispatcher.InvokeAsync(async () => Autostereogram = await ToBitmapImageAsync(autostereogram));
        }
        finally
        {
            patternImage?.Dispose();
        }
    }

    [RelayCommand]
    public void Loaded(RoutedEventArgs e)
    {
        var frameworkElement = (FrameworkElement)e.Source;
        Dpi = (int)VisualTreeHelper.GetDpi(frameworkElement).PixelsPerInchX;
        Width =  (int)frameworkElement.ActualWidth;
        Height = (int)frameworkElement.ActualHeight;
    }

    [RelayCommand]
    public void SizeChanged(SizeChangedEventArgs e)
    {
        Width = (int)e.NewSize.Width;
        Height = (int)e.NewSize.Height;
        SizeChangedTimer.Change(TimeSpan.FromSeconds(0.5), TimeSpan.Zero);
    }
}
