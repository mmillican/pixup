using Amazon.S3;
using Amazon.S3.Transfer;
using Pixup.CommandLine.Models.Config;

namespace Pixup.CommandLine.Storage;

public class S3StorageProvider : IStorageProvider
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3BucketConfiguration _config;

    public S3StorageProvider(S3BucketConfiguration config)
    {
        _config = config;
        _s3Client = new AmazonS3Client();
    }

    public async Task SaveFile(FileInfo file)
    {
        var fileTransferUtility = new TransferUtility(_s3Client);
        
        var keyName = GetS3KeyName(file);
        
        Console.WriteLine($"Uploading {file.Name} to {_config.BucketName}/{keyName}");
        
        await fileTransferUtility.UploadAsync(file.FullName, _config.BucketName, keyName);
        
        Console.WriteLine($"...uploaded {file.Name}");
    }

    public async Task SaveFiles(IEnumerable<FileInfo> files)
    {
        var tasks = files.Select(SaveFile);
        await Task.WhenAll(tasks);
    }
    
    private string GetS3KeyName(FileInfo file)
    {
        // Combine path prefix with filename, ensuring proper path formatting
        return string.IsNullOrEmpty(_config.PathPrefix) 
            ? file.Name 
            : $"{_config.PathPrefix.TrimEnd('/')}/{file.Name}";
    }   
}