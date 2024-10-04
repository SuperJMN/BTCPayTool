using CommandLine;

namespace BTCPayTool;

[Verb("new-plugin", HelpText = "Crear un nuevo plugin.")]
class NewPluginOptions
{
    [Option("name", Default = "MyPlugin", HelpText = "Plugin name")]
    public string Name { get; set; }
}