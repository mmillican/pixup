namespace Pixup.CommandLine.Storage;

public interface IStorageProvider
{
    Task SaveFile(FileInfo file);
    Task SaveFiles(IEnumerable<FileInfo> files);
}