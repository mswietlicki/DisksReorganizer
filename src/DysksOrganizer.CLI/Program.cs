using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.IO;

namespace DysksOrganizer.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            var greeting = new Command("greeting", "Say hi.")
            {
                new Argument<string>("name", "Your name."),
                new Option<string?>(new[] { "--greeting", "-g" }, "The greeting to use."),
                new Option(new[] { "--verbose", "-v" }, "Show the deets."),
            };

            greeting.Handler = CommandHandler.Create<string, string?, bool, IConsole>(HandleGreeting);

            var cmd = new RootCommand
            {
                greeting
            };

            return cmd.Invoke(args);
        }

        static void HandleGreeting(string name, string? greeting, bool verbose, IConsole console)
        {
            if (verbose)
                console.Out.WriteLine($"About to say hi to '{name}'...");

            greeting ??= "Hi";
            console.Out.WriteLine($"{greeting} {name}!");

            if (verbose)
                console.Out.WriteLine($"All done!");
        }
    }

    public class Disk
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string Description { get; set; }
        public int Manufacture { get; set; }
        public string SerialNumber { get; set; }
    }

    public class FileSource
    {
        public string Name { get; set; }
        public SourcePath Path { get; set; }
    }

    public class SourcePath
    {
        public string Path { get; set; }
    }

    public class File
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string Hash { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public SourcePath Path { get; set; }
    }
}
