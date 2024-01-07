namespace Catalyst.Engine.Core;

public class PropertyChangedEventArgs
{
    public string PropertyName { get; }
    
    public PropertyChangedEventArgs(string propertyName)
    {
        PropertyName = propertyName;
    }
}