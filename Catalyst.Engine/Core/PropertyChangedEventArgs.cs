namespace Catalyst.Engine.Core;

public class PropertyChangedEventArgs
{
    public string PropertyName { get; }
    public object? OldValue { get; }
    public object? NewValue { get; }
    
    public PropertyChangedEventArgs(string propertyName, object? oldValue, object? newValue)
    {
        PropertyName = propertyName;
        OldValue = oldValue;
        NewValue = newValue;
    }
}