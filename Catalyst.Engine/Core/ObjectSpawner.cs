using System;
using System.Collections.Generic;

namespace Catalyst.Engine.Core;

/// <summary>
/// Handles object spawning and despawning.
/// </summary>
public class ObjectSpawner
{
    public IEnumerable<ILevelObject> ActiveObjects => activeObjects;

    private readonly List<ILevelObject> activateList = new List<ILevelObject>();
    private readonly List<ILevelObject> deactivateList = new List<ILevelObject>();

    private int activateIndex = 0;
    private int deactivateIndex = 0;
    private float currentTime = 0.0f;

    private readonly HashSet<ILevelObject> activeObjects = new HashSet<ILevelObject>();

    public ObjectSpawner(IEnumerable<ILevelObject> levelObjects)
    {
        // populate activate and deactivate lists
        activateList.AddRange(levelObjects);
        deactivateList.AddRange(activateList);

        // sort by start time
        activateList.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));

        // sort by kill time
        deactivateList.Sort((a, b) => a.KillTime.CompareTo(b.KillTime));
    }

    public void Update(float time)
    {
        if (time >= currentTime)
        {
            UpdateObjectsForward(time);
        }
        else
        {
            UpdateObjectsBackward(time);
        }
        
        currentTime = time;
    }

    /// <summary>
    /// Insert one object only.
    /// </summary>
    /// <param name="levelObject">The object to insert into.</param>
    /// <param name="recalculate">Whether should this recalculate object states.</param>
    public void InsertObject(ILevelObject levelObject, bool recalculate = true)
    {
        activateList.Add(levelObject);
        activateList.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
        
        deactivateList.Add(levelObject);
        deactivateList.Sort((a, b) => a.KillTime.CompareTo(b.KillTime));

        if (recalculate)
        {
            RecalculateObjectStates();
        }
    }
    
    /// <summary>
    /// Remove one object only.
    /// </summary>
    /// <param name="levelObject">The object to remove from.</param>
    /// <param name="recalculate">Whether should this recalculate object states.</param>
    public void RemoveObject(ILevelObject levelObject, bool recalculate = true)
    {
        activateList.Remove(levelObject);
        deactivateList.Remove(levelObject);

        if (recalculate)
        {
            RecalculateObjectStates();
        }
    }
    
    /// <summary>
    /// Insert multiple objects.
    /// </summary>
    /// <param name="levelObjects">The list of objects to insert into.</param>
    /// <param name="recalculate">Whether should this recalculate object states.</param>
    public void InsertObjects(IEnumerable<ILevelObject> levelObjects, bool recalculate = true)
    {
        activateList.AddRange(levelObjects);
        activateList.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
        
        deactivateList.AddRange(levelObjects);
        deactivateList.Sort((a, b) => a.KillTime.CompareTo(b.KillTime));

        if (recalculate)
        {
            RecalculateObjectStates();
        }
    }

    /// <summary>
    /// Remove multiple objects.
    /// </summary>
    /// <param name="predicate">The predicate that matches the objects to remove.</param>
    /// <param name="recalculate">Whether should this recalculate object states.</param>
    public void RemoveObjects(Predicate<ILevelObject> predicate, bool recalculate = true)
    {
        activateList.RemoveAll(predicate);
        deactivateList.RemoveAll(predicate);

        if (recalculate)
        {
            RecalculateObjectStates();
        }
    }

    /// <summary>
    /// Clear all objects.
    /// </summary>
    public void Clear()
    {
        activateIndex = 0;
        deactivateIndex = 0;
        currentTime = 0.0f;
        activeObjects.Clear();
        activateList.Clear();
        deactivateList.Clear();
    }

    /// <summary>
    /// Recalculate object states.
    /// </summary>
    public void RecalculateObjectStates()
    {
        activateIndex = 0;
        deactivateIndex = 0;
        activeObjects.Clear();
        UpdateObjectsForward(currentTime);
    }

    private void UpdateObjectsForward(float time)
    {
        // Spawn
        while (activateIndex < activateList.Count && time >= activateList[activateIndex].StartTime)
        {
            activateList[activateIndex].SetActive(true);
            activeObjects.Add(activateList[activateIndex]);
            activateIndex++;
        }

        // Despawn
        while (deactivateIndex < deactivateList.Count && time >= deactivateList[deactivateIndex].KillTime)
        {
            deactivateList[deactivateIndex].SetActive(false);
            activeObjects.Remove(deactivateList[deactivateIndex]);
            deactivateIndex++;
        }
    }

    private void UpdateObjectsBackward(float time)
    {
        // Spawn (backwards)
        while (deactivateIndex - 1 >= 0 && time < deactivateList[deactivateIndex - 1].KillTime)
        {
            deactivateList[deactivateIndex - 1].SetActive(true);
            activeObjects.Add(deactivateList[deactivateIndex - 1]);
            deactivateIndex--;
        }

        // Despawn (backwards)
        while (activateIndex - 1 >= 0 && time < activateList[activateIndex - 1].StartTime)
        {
            activateList[activateIndex - 1].SetActive(false);
            activeObjects.Remove(activateList[activateIndex - 1]);
            activateIndex--;
        }
    }
}