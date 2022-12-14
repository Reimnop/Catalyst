using Catalyst.Engine.Core;

namespace Catalyst.Engine.Data;

/// <summary>
/// Contains level data.
/// </summary>
public class Level
{
    public IReadOnlyList<ILevelObject> Objects => objects.AsReadOnly();

    private List<ILevelObject> objects;
    
    public event EventHandler<ILevelObject>? ObjectInserted;
    public event EventHandler<ILevelObject>? ObjectRemoved;

    public Level()
    {
        objects = new List<ILevelObject>();
    }

    public Level(IEnumerable<ILevelObject> levelObjects)
    {
        objects = levelObjects.ToList();
    }
    
    public void InsertObject(ILevelObject levelObject)
    {
        objects.Add(levelObject);
        ObjectInserted?.Invoke(this, levelObject);
    }
    
    public void RemoveObject(ILevelObject levelObject)
    { 
        objects.Remove(levelObject);
        ObjectRemoved?.Invoke(this, levelObject);
    }
}