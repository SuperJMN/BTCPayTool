using CSharpFunctionalExtensions;

namespace BTCPayTool;

public interface IGitClient
{
    Result AddSubmodule(string name, Uri uri);
    Result Init();
}