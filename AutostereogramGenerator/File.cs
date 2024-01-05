namespace AutostereogramGenerator;

/// <summary>
/// File
/// </summary>
/// <param name="path">file path</param>
public class File(string path)
{
    /// <summary>
    /// Gets the fileName
    /// </summary>
    public string Name { get; } = System.IO.Path.GetFileNameWithoutExtension(path);

    /// <summary>
    /// Gets the file path
    /// </summary>
    public string Path { get; } = path;
}
