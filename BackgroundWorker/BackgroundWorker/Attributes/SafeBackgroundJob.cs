namespace BackgroundWorker.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class SafeBackgroundJobAttribute(int retryDelayMilliseconds = 5000) : Attribute
{
    public TimeSpan RetryDelay { get; } = TimeSpan.FromMilliseconds(retryDelayMilliseconds);
}
