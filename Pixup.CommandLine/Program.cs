// See https://aka.ms/new-console-template for more information

using CommandLine;
using Pixup.CommandLine;

Console.WriteLine("Pixup here!");

await Parser.Default.ParseArguments<PixupOptions>(args)
    .WithParsedAsync(async options =>
    {
        var processor = new PixupProcessor(options);
        
        await processor.RunAsync();
    });
    