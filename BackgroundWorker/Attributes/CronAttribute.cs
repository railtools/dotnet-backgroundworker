namespace BackgroundWorker.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class CronAttribute(string expression) : Attribute
{
    public string Expression { get; } = expression;
}