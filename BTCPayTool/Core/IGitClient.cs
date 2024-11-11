namespace BTCPayTool.Core;

public interface IGitClient
{
    Task AddSubmodule(string name, Uri uri);
    Task Init();
}