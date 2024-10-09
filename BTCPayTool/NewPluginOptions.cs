using CommandLine;

namespace BTCPayTool;

[Verb("new-plugin", HelpText = "Create new plugin")]
internal class NewPluginOptions
{
    [Option("name", Default = "MyPlugin", HelpText = "Plugin name")]
    public string Name { get; set; }
}