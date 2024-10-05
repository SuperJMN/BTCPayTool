namespace BTCPayTool.Core;

public interface IGitClient
{
    Result AddSubmodule(string name, Uri uri);
    Result Init();
}