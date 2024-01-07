using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Autostereogram.Tests;

public class AutostereogramGeneratorTests
{
    private const string SHARK = "Shark.png";
    private const string ROCKS = "Rocks.png";

    private static Image<Rgb24> LoadImage(string image)
    {
        return Image.Load<Rgb24>(image);
    }

    private static void CreateImage(string depthMapPath, string? patternPath, string saveFileName, int width = 0, int height = 0, int dpi = 96,
        int observatorDistance = 24)
    {
        using var depthMap = LoadImage(depthMapPath);
        using var pattern = patternPath == null ? null : LoadImage(patternPath);
        var autostereogram = new AutoStereogramGenerator(depthMap, pattern, width, height, dpi, observatorDistance).Create();
#if DEBUG
        autostereogram.SaveAsPng(saveFileName);
#endif
        Assert.NotNull(autostereogram);
    }

    [Fact]
    public static void CreateAutostereogram()
    {
        CreateImage(SHARK, ROCKS, "Autostereogram.png", 1920, 1080);
    }

    [Fact]
    public static void CreateAutostereogramForPrinting()
    {
        CreateImage(SHARK, ROCKS, "AutostereogramPrint.png", 1123, 794, 96, 16);
    }

    [Fact]
    public static void CreateSIRDS()
    {
        CreateImage(SHARK, null, "SIRDS.png");
    }
}