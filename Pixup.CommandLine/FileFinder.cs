namespace Pixup.CommandLine;

public class FileFinder
{
    private readonly string _path;
    private readonly bool _recursive;
    
    public FileFinder(string path, bool recursive)
    {
        _path = path;
        _recursive = recursive;
    }

    public List<FileInfo> FindFiles(string? extension = null)
    {
        var directoryInfo = new DirectoryInfo(_path);
        
        var searchPattern = !string.IsNullOrEmpty(extension) ? $"*{extension}" : "*";
        
        var files = directoryInfo.GetFiles(searchPattern, _recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        return files.ToList();
    }

    public List<FileInfo> FindFiles(string[] extensions)
    {
        var result = new List<FileInfo>();
        foreach (var ext in extensions)
        {
            result.AddRange(FindFiles(ext));
        }

        return result;
    }
}