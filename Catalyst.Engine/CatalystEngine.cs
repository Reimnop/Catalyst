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
    private readonly ILevelView levelView;

    public CatalystEngine(ILevelView levelView)
    {
        this.levelView = levelView;
        objectSpawner = new ObjectSpawner(levelView);
        
        levelView.ObjectInserted += OnObjectInserted;
        levelView.ObjectRemoved += OnObjectRemoved;
    }

    ~CatalystEngine()
    {
        Dispose(false);
    }

    public void Update(float time)
    {
        objectSpawner.Update(time);

        foreach (ILevelObject levelObject in objectSpawner.ActiveObjects)
        {
            levelObject.UpdateTime(time);
        }
    }

    private void OnObjectInserted(object sender, ILevelObject e)
    {
        objectSpawner.QueueInsertObject(e);
    }
    
    private void OnObjectRemoved(object sender, ILevelObject e)
    {
        objectSpawner.QueueRemoveObject(e);
    }

    public void Dispose()
    {
        Dispose(true);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        levelView.ObjectInserted -= OnObjectInserted;
        levelView.ObjectRemoved -= OnObjectRemoved;
    }
}