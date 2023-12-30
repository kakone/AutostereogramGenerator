using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Autostereogram;

/// <summary>
/// Autostereogram generator
/// </summary>
/// <param name="depthMap">depth map</param>
/// <param name="pattern">pattern (or null to create a SIRDS)</param>
/// <param name="width">result width</param>
/// <param name="height">result height</param>
/// <param name="dpi">dots per inch number of the display device</param>
/// <param name="observatorDistance">observator distance in inchs</param>
public class AutoStereogramGenerator(Image<Rgb24> depthMap, Image<Rgb24>? pattern = null, int width = 0, int height = 0, int dpi = 72, int observatorDistance = 24)
{
    /// <summary>
    /// Gets or sets the depth map
    /// </summary>
    public Image<Rgb24> DepthMap { get; set; } = depthMap;

    /// <summary>
    /// Gets or sets the pattern
    /// </summary>
    public Image<Rgb24>? Pattern { get; set; } = pattern;

    /// <summary>
    /// Gets or sets the result width
    /// </summary>
    public int Width { get; set; } = width;

    /// <summary>
    /// Gets or sets the result height
    /// </summary>
    public int Height { get; set; } = height;

    /// <summary>
    /// Gets or sets the dots per inch number of the display device
    /// </summary>
    public int Dpi { get; set; } = dpi;

    /// <summary>
    /// Gets or sets the observator distance in inchs
    /// </summary>
    public int ObservatorDistance { get; set; } = observatorDistance;

    /// <summary>
    /// Creates the autostereogram
    /// </summary>
    /// <returns>autostereogram image</returns>
    public Image<Rgb24> Create()
    {
        var eyeSep = Dpi * 2.5;
        var eyeSepMid = eyeSep / 2;
        var obsDist = Dpi * ObservatorDistance;
        var sepFactor = 0.55;   // Ratio of the smallest allowable separation to the maximum separation used

        int depthMapWidth;
        var width = Width;
        var height = Height;
        if (width <= 0)
        {
            depthMapWidth = DepthMap.Width;
            width =(int)Math.Ceiling(depthMapWidth + eyeSep);
        }
        else
        {
            depthMapWidth = (int)(width - eyeSep);
        }
        if (height <= 0)
        {
            height = DepthMap.Height;
        }
        if (depthMapWidth != DepthMap.Width || height != DepthMap.Height)
        {
            DepthMap.Mutate(x => x.Resize(new ResizeOptions() { Mode = ResizeMode.Max, Size = new Size(depthMapWidth, height) }));
            depthMapWidth = DepthMap.Width;
        }
        var depthMapHeight = DepthMap.Height;
        var depthMapPosX = (width - DepthMap.Width) / 2;
        var depthMapPosY = (height - DepthMap.Height) / 2;

        var autoStereogram = new Image<Rgb24>(width, height);
        var maxDepth = obsDist;
        var minDepth = sepFactor * maxDepth * obsDist / ((1 - sepFactor) * maxDepth + obsDist);
        var depthFactor = (maxDepth - minDepth) / 256;
        var linkedLeftPoints = new int[width];
        var linkedRightPoints = new int[width];
        int leftPoint, rightPoint, left, right, x, depthMapX, depthMapY;
        byte depthMapValue;
        double depth, sepMid;
        var random = new Random();

        for (var y = 0; y < height; y++)
        {
            for (x = 0; x < width; x++)
            {
                // reset the links arrays
                linkedLeftPoints[x] = -1;
                linkedRightPoints[x] = -1;
            }

            for (x = 0; x < width; x++)
            {
                depthMapX = x - depthMapPosX;
                depthMapY = y - depthMapPosY;
                depthMapValue = (depthMapX < 0  || depthMapX >= depthMapWidth || depthMapY < 0 || depthMapY >= depthMapHeight) ? (byte)0 :
                    DepthMap[depthMapX, depthMapY].R;
                depth = maxDepth - depthMapValue * depthFactor;
                sepMid = eyeSepMid * depth / (depth + obsDist);
                left = (int)Math.Round(x - sepMid);
                right = (int)Math.Round(x + sepMid);

                if (left < 0 || right >= width)
                {
                    continue;
                }

                leftPoint = linkedLeftPoints[right];
                if (leftPoint != -1) // right point is already linked
                {
                    if (left <= leftPoint) // current is deeper
                    {
                        continue;
                    }
                    linkedRightPoints[leftPoint] = -1; // break old link
                }

                rightPoint = linkedRightPoints[left];
                if (rightPoint != -1) // left point is already linked
                {
                    if (right >= rightPoint) // current is deeper
                    {
                        continue;
                    }
                    linkedLeftPoints[rightPoint] = -1; // break old link
                }

                // make link
                linkedLeftPoints[right] = left;
                linkedRightPoints[left] = right;
            }

            for (x = 0; x < width; x++)
            {
                autoStereogram[x, y] =
                    linkedLeftPoints[x] == -1 ?
                      // unconstrained
                      (Pattern == null ?
                        new Rgb24((byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255)) :
                        Pattern[x % Pattern.Width, y % Pattern.Height]) :
                      // constrained
                      autoStereogram[linkedLeftPoints[x], y];
            }
        }

        return autoStereogram;
    }
}
