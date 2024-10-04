using CSharpFunctionalExtensions;

namespace BTCPayTool;

public static class ResultExtensions
{
    public static Result<T> Using<TDisposable, T>(Func<TDisposable> factory, Func<TDisposable, Result<T>> func)
        where TDisposable : IDisposable
    {
        using (var disposable = factory())
        {
            return func(disposable);
        }
    }
    
    public static async Task<Result<T>> Using<TDisposable, T>(Func<Task<TDisposable>> factory, Func<TDisposable, Task<Result<T>>> func)
        where TDisposable : IDisposable
    {
        using (var disposable = await factory())
        {
            return await func(disposable);
        }
    }
    
    public static async Task<Result> Using<TDisposable>(Func<Task<TDisposable>> factory, Func<TDisposable, Task<Result>> func)
        where TDisposable : IDisposable
    {
        using (var disposable = await factory())
        {
            return await func(disposable);
        }
    }
}