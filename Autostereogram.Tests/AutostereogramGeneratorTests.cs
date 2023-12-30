using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Autostereogram.Tests;

public class AutostereogramGeneratorTests
{
    private const string SHARK = "Shark.png";
    private const string MARBLE = "Marble.png";

    private static Image<Rgb24> LoadImage(string image)
    {
        return Image.Load<Rgb24>(image);
    }

    [Fact]
    public void CreateAutostereogram()
    {
        using var depthMap = LoadImage(SHARK);
        using var pattern = LoadImage(MARBLE);
        var autostereogram = new AutoStereogramGenerator(depthMap, pattern, 1600, 900).Create();
#if DEBUG
        autostereogram.SaveAsPng("Autostereogram.png");
#endif
        Assert.NotNull(autostereogram);
    }

    [Fact]
    public void CreateSIRDS()
    {
        using var depthMap = LoadImage(SHARK);
        var sirds = new AutoStereogramGenerator(depthMap, null, 1600, 900).Create();
#if DEBUG
        sirds.SaveAsPng("SIRDS.png");
#endif
        Assert.NotNull(sirds);
    }
}