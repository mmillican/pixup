using Pixup.CommandLine.Models.Config;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Pixup.CommandLine.Images;

public class ImageProcessor
{
    private readonly ResizeConfiguration _resizeConfiguration;

    public ImageProcessor(ResizeConfiguration resizeConfiguration)
    {
        _resizeConfiguration = resizeConfiguration;
    }

    public async Task ProcessImageAsync(FileInfo sourceFile)
    {
        if (!_resizeConfiguration.Enabled || !IsResizeable(sourceFile))
        {
            return;
        }
        
        // Create the output directory if it doesn't exist
        var outputDirectory = Path.Combine(
            Path.GetDirectoryName(sourceFile.FullName) ?? "",
            _resizeConfiguration.OutputPath);
        
        Directory.CreateDirectory(outputDirectory);
        
        using var image = await Image.LoadAsync(sourceFile.FullName);
        foreach (var variant in _resizeConfiguration.Variants)
        {
            using var variantImg = image.Clone(x =>
            {
                x.Resize(new ResizeOptions
                {
                    Size = new Size(variant.Width, variant.Height),
                    Mode = ResizeMode.Max,
                });
            });
            
            var origFileName = Path.GetFileNameWithoutExtension(sourceFile.Name);
            var outputFileName = $"{origFileName}-{variant.Key}{Path.GetExtension(sourceFile.Name)}";
            var outputPath = Path.Combine(outputDirectory, outputFileName);
            await variantImg.SaveAsync(outputPath);
        }
    }

    public async Task ProcessImagesAsync(IEnumerable<FileInfo> files)
    {
        var startTime = DateTime.Now;
        
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount / 4,
        };

        await Parallel.ForEachAsync(files, parallelOptions, async (file, _) =>
        {
            await ProcessImageAsync(file);
        });
        
        var endTime = DateTime.Now;
        var elapsedTime = endTime - startTime;
        Console.WriteLine($"Processed {files.Count()} images in {elapsedTime.TotalMilliseconds} milliseconds.");
    }

    private static bool IsResizeable(FileInfo file)
    {
        string[] resizeableExtensions = [ ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff" ];
        return resizeableExtensions.Contains(file.Extension);
    }
}