using System;
using Catalyst.Engine;
using Catalyst.Engine.Core;
using Catalyst.Engine.Data;

using GameData = DataManager.GameData;

namespace Catalyst.Logic;

public class LevelProcessor : IDisposable
{
    private readonly CatalystEngine engine;

    public LevelProcessor(GameData gameData)
    {
        // Convert GameData to LevelObjects
        var converter = new GameDataLevelObjectsConverter(gameData);
        var levelObjects = converter.ToLevelObjects();

        var level = new Level<ILevelObject>(levelObjects);
        engine = new CatalystEngine(level.View);

        CatalystBase.LogInfo($"Loaded {level.Objects.Count} objects (original: {gameData.beatmapObjects.Count})");
    }

    public void Update(float time)
    {
        engine.Update(time);
    }

    public void Dispose()
    {
        engine.Dispose();
    }
}