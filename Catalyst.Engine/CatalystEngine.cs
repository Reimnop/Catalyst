using System;
using Catalyst.Engine.Core;
using Catalyst.Engine.Data;

namespace Catalyst.Engine;

/// <summary>
/// Main animation engine class.
/// </summary>
public class CatalystEngine : IDisposable
{
    private readonly ObjectSpawner objectSpawner;
    private readonly Level level;

    public CatalystEngine(Level level)
    {
        this.level = level;
        objectSpawner = new ObjectSpawner(level.Objects);
        
        level.ObjectInserted += OnObjectInserted;
        level.ObjectRemoved += OnObjectRemoved;
    }

    public void Update(float time)
    {
        objectSpawner.Update(time);

        foreach (ILevelObject levelObject in objectSpawner.ActiveObjects)
        {
            levelObject.Interpolate(time);
        }
    }

    private void OnObjectInserted(object sender, ILevelObject e)
    {
        objectSpawner.InsertObject(e);
    }
    
    private void OnObjectRemoved(object sender, ILevelObject e)
    {
        objectSpawner.RemoveObject(e);
    }

    public void Dispose()
    {
        level.ObjectInserted -= OnObjectInserted;
        level.ObjectRemoved -= OnObjectRemoved;
    }
}