using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Pixup.CommandLine.Models.Config;

public class ProcessingConfiguration
{
	private const int MaxParentPathsToTraverse = 2;
	
	public string SourcePath { get; set; } = "./";
	
	public StorageConfiguration Storage { get; set; } = new();

	public ResizeConfiguration Resize { get; set; } = new();

	public static ProcessingConfiguration LoadConfiguration(string path, string fileName)
	{
		var directoryInfo = new DirectoryInfo(path);

		for (var idx = 0; idx <= MaxParentPathsToTraverse; idx++)
		{
			var configFilePath = Path.Combine(directoryInfo.FullName, fileName);
			Console.WriteLine($"Looking for config file at: {configFilePath}");

			if (File.Exists(configFilePath))
			{
				var configFileContent = File.ReadAllText(configFilePath);
				return ParseConfig(configFileContent);
			}

			if (directoryInfo.Parent is null)
			{
				Console.WriteLine("Reached root directory");
				break;
			}
			
			directoryInfo = directoryInfo.Parent;
		}
        
		// TODO: Consider looking at the user's profile
        
		throw new FileNotFoundException($"Could not find {fileName} in the directory or {MaxParentPathsToTraverse} levels up.");
	}
	
	private static ProcessingConfiguration ParseConfig(string content)
	{
		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(UnderscoredNamingConvention.Instance)
			.IgnoreUnmatchedProperties()
			.Build();
    
		return deserializer.Deserialize<ProcessingConfiguration>(content);
	}
}

public class StorageConfiguration
{
	public string Provider { get; set; } = "s3";
	
	public S3BucketConfiguration S3 { get; set; }
}

public class S3BucketConfiguration
{
	public string BucketName { get; set; } = "";
	public string PathPrefix { get; set; } = "";
}

public class ResizeConfiguration
{
	public bool Enabled { get; set; }

	public string[] FileTypes { get; set; } = [];
	
	public List<ImageVariant> Variants { get; set; } = [];
	public string OutputPath { get; set; } = "./output";
}

public class ImageVariant
{
	public string Key { get; set; } = "";
	public int Width { get; set; }
	public int Height { get; set; }
	// TODO: Add mode
}