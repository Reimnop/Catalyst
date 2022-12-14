using System;
using System.Collections.Generic;
using Catalyst.Engine;
using Catalyst.Engine.Core;
using Catalyst.Engine.Data;

using GameData = DataManager.GameData;

namespace Catalyst.Logic;

public class LevelProcessor : IDisposable
{
    private readonly Level level;
    private readonly CatalystEngine engine;

    public LevelProcessor(GameData gameData)
    {
        // Convert GameData to LevelObjects
        GameDataLevelObjectsConverter converter = new GameDataLevelObjectsConverter(gameData);
        IEnumerable<ILevelObject> levelObjects = converter.ToLevelObjects();

        level = new Level(levelObjects);
        engine = new CatalystEngine(level);

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