using Pixup.CommandLine.Images;
using Pixup.CommandLine.Models.Config;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Pixup.CommandLine;

public class PixupProcessor
{
    private const int MaxParentPathsToTraverse = 2;
    
    private readonly PixupOptions _options;

    public PixupProcessor(PixupOptions options)
    {
        _options = options;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Starting Pixup processor...");

        var workingPath = NormalizePath(_options.WorkingDirectory);
        Console.WriteLine($"Working directory: {workingPath}");
        
        var config = ProcessingConfiguration.LoadConfiguration(workingPath, _options.ConfigFileName);

        // Note: this won't work when the config's `source_path` into account. Need to figure out how to handle that
        var sourcePath = workingPath;
        
        var fileFinder = new FileFinder(sourcePath, false);

        var filesToProcess = config.Resize.FileTypes.Length != 0
            ? fileFinder.FindFiles(config.Resize.FileTypes) 
            : fileFinder.FindFiles();
        
        Console.WriteLine($"Found {filesToProcess.Count} files to process...");
        
        var imageProcessor = new ImageProcessor(config.Resize);
        await imageProcessor.ProcessImagesAsync(filesToProcess);
    }
    
    private static string NormalizePath(string path)
    {
        if (Path.IsPathRooted(path))
        {
            return Path.GetFullPath(path);
        }
        
        if (path.StartsWith('~'))
        {
            Console.WriteLine("User profile path");
            
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            
            var relativePath = path.StartsWith("~/")
                ? path[2..]
                : path[1..];
            
            Console.WriteLine("Relative path: " + relativePath);
            
            return Path.GetFullPath(Path.Combine(userProfile, relativePath));
        }
        
        var resolvedPath = Path.Combine(Directory.GetCurrentDirectory(), path);
        return Path.GetFullPath(resolvedPath);
    }
}