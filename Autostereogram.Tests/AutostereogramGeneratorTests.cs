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

    private static void CreateImage(string depthMapPath, string? patternPath, string saveFileName, int width, int height, int dpi = 96,
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
    public void CreateAutostereogram()
    {
        CreateImage(SHARK, ROCKS, "Autostereogram.png", 1600, 900);
    }

    [Fact]
    public void CreateAutostereogramForPrinting()
    {
        CreateImage(SHARK, ROCKS, "AutostereogramPrint.png", 3508, 2480, 300, 16);
    }

    [Fact]
    public void CreateSIRDS()
    {
        CreateImage(SHARK, null, "SIRDS.png", 1600, 900);
    }
}