using CommandLine;

namespace Pixup.CommandLine;

public class PixupOptions
{
    [Option(
        'd',
        "directory",
        HelpText = "The working directory for processing files. Defaults to the current directory.")]
    public string WorkingDirectory { get; set; } = "./";
    
    [Option(
            'c',
            "config-file", 
            HelpText = "The name of the config file to use for processing. Defaults to 'pixup.yaml'")
    ]
    public string ConfigFileName { get; set; } = "pixup.yaml";
}