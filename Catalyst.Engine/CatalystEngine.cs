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
    }
    
    ~CatalystEngine()
    {
        Dispose();
    }

    public void Update(float time)
    {
        objectSpawner.Update(time);

        foreach (var levelObject in objectSpawner.ActiveObjects)
        {
            levelObject.UpdateTime(time);
        }
    }

    public void Dispose()
    {
        objectSpawner.Dispose();
        GC.SuppressFinalize(this);
    }
}