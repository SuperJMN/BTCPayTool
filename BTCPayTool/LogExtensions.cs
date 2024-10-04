namespace BTCPayTool;

public static class LogExtensions
{
    public static bool TapTrue(this bool b, Action action)
    {
        if (b)
        {
            action();
        }
        
        return b;
    }
}