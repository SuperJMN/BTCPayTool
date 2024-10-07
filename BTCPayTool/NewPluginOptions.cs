using CommandLine;

namespace BTCPayTool;

[Verb("new-plugin", HelpText = "Create new plugin")]
internal class NewPluginOptions
{
    [Option("name", Default = "MyPlugin", HelpText = "Plugin name")]
    public string Name { get; set; }
}

[Verb("initialize-plugin-solution", HelpText = "Creates a plugin solution where you can have one or more plugins")]
internal class InitializePluginSolutionOptions
{
    [Option("name", Default = "MyPlugins", HelpText = "Plugin solution name")]
    public string Name { get; set; }
}