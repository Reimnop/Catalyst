namespace Catalyst.Engine.Data;

/// <summary>
/// Contains level data.
/// </summary>
public class Level
{
    public IReadOnlyList<LevelObject> Objects => objects.AsReadOnly();

    private List<LevelObject> objects = new List<LevelObject>();
    
    public event EventHandler<LevelObject>? ObjectInserted;
    public event EventHandler<LevelObject>? ObjectRemoved;
    
    public void InsertObject(LevelObject levelObject)
    {
        objects.Add(levelObject);
        ObjectInserted?.Invoke(this, levelObject);
    }
    
    public void RemoveObject(LevelObject levelObject)
    {
        objects.Remove(levelObject);
        ObjectRemoved?.Invoke(this, levelObject);
    }
}