namespace AutostereogramGenerator;

public class File(string path)
{
    public string Name { get; } = System.IO.Path.GetFileNameWithoutExtension(path);
    public string Path { get; } = path;
}
