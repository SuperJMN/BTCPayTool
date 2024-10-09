using CommandLine;

namespace BTCPayTool;

[Verb("init-sln", HelpText = "Creates a plugin solution where you can have one or more plugins")]
internal class InitializePluginSolutionOptions
{
    [Option("name", Default = "Plugins", HelpText = "Plugin solution name")]
    public string Name { get; set; }
}