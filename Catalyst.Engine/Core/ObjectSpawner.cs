using System;
using System.Collections.Generic;

namespace Catalyst.Engine.Core;

/// <summary>
/// Handles object spawning and despawning.
/// </summary>
public class ObjectSpawner
{
    public IEnumerable<ILevelObject> ActiveObjects => activeObjects;

    private readonly List<ILevelObject> activateList = new();
    private readonly List<ILevelObject> deactivateList = new();

    private int activateIndex = 0;
    private int deactivateIndex = 0;
    private float currentTime = 0.0f;

    private readonly HashSet<ILevelObject> activeObjects = new();
    
    // Queues
    private readonly HashSet<ILevelObject> insertQueue = new();
    private readonly HashSet<ILevelObject> removeQueue = new();

    public ObjectSpawner(IEnumerable<ILevelObject> levelObjects)
    {
        // populate activate and deactivate lists
        activateList.AddRange(levelObjects);
        deactivateList.AddRange(activateList);

        // sort by start time
        activateList.Sort(CompareStartTime);

        // sort by kill time
        deactivateList.Sort(CompareKillTime);
    }

    public void Update(float time)
    {
        if (insertQueue.Count > 0)
        {
            foreach (var levelObject in insertQueue)
            {
                activateList.Add(levelObject);
                deactivateList.Add(levelObject);
                
                if (currentTime >= levelObject.StartTime && currentTime < levelObject.KillTime)
                {
                    levelObject.EnterLevel();
                    activeObjects.Add(levelObject);
                }
            }
            
            // Clear queue
            insertQueue.Clear();
            
            // Sort
            activateList.Sort(CompareStartTime);
            deactivateList.Sort(CompareKillTime);
            
            // Recalculate indices
            activateIndex = CalculateIndex(time, activateList, x => x.StartTime);
            deactivateIndex = CalculateIndex(time, deactivateList, x => x.KillTime);
        }
        
        if (removeQueue.Count > 0)
        {
            foreach (var levelObject in removeQueue)
            {
                activateList.Remove(levelObject);
                deactivateList.Remove(levelObject);
                
                if (activeObjects.Contains(levelObject))
                {
                    levelObject.ExitLevel();
                    activeObjects.Remove(levelObject);
                }
            }
            
            // Clear queue
            removeQueue.Clear();
            
            // We don't need to sort here, since we only remove objects,
            // which means that the lists are still sorted.
            
            // Recalculate indices
            activateIndex = CalculateIndex(time, activateList, x => x.StartTime);
            deactivateIndex = CalculateIndex(time, deactivateList, x => x.KillTime);
        }
        
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
    /// Add an object to insert queue. Will get inserted on next update.
    /// </summary>
    /// <param name="levelObject">The object to insert into.</param>
    public void QueueInsertObject(ILevelObject levelObject)
    {
        insertQueue.Add(levelObject);
    }
    
    /// <summary>
    /// Add an object to remove queue. Will get removed on next update.
    /// </summary>
    /// <param name="levelObject">The object to remove from.</param>
    public void QueueRemoveObject(ILevelObject levelObject)
    {
        removeQueue.Add(levelObject);
    }

    private void UpdateObjectsForward(float time)
    {
        // Spawn
        while (activateIndex < activateList.Count && time >= activateList[activateIndex].StartTime)
        {
            activateList[activateIndex].EnterLevel();
            activeObjects.Add(activateList[activateIndex]);
            activateIndex++;
        }

        // Despawn
        while (deactivateIndex < deactivateList.Count && time >= deactivateList[deactivateIndex].KillTime)
        {
            deactivateList[deactivateIndex].ExitLevel();
            activeObjects.Remove(deactivateList[deactivateIndex]);
            deactivateIndex++;
        }
    }

    private void UpdateObjectsBackward(float time)
    {
        // Spawn (backwards)
        while (deactivateIndex - 1 >= 0 && time < deactivateList[deactivateIndex - 1].KillTime)
        {
            deactivateList[deactivateIndex - 1].EnterLevel();
            activeObjects.Add(deactivateList[deactivateIndex - 1]);
            deactivateIndex--;
        }

        // Despawn (backwards)
        while (activateIndex - 1 >= 0 && time < activateList[activateIndex - 1].StartTime)
        {
            activateList[activateIndex - 1].ExitLevel();
            activeObjects.Remove(activateList[activateIndex - 1]);
            activateIndex--;
        }
    }
    
    private static int CalculateIndex(float time, IReadOnlyList<ILevelObject> list, Func<ILevelObject, float> propertyGetter)
    {
        // Exit early
        if (list.Count == 0)
            return 0;
        
        if (list.Count == 1)
        {
            var value = propertyGetter(list[0]);
            return time >= value ? 1 : 0;
        }
        
        var left = 0;
        var right = list.Count;

        while (left < right)
        {
            var mid = (left + right) / 2;
            var valueLeft = propertyGetter(list[mid - 1]);
            var valueRight = propertyGetter(list[mid]);
            if (time >= valueLeft && time < valueRight)
                return mid;
            if (time < valueLeft)
                right = mid - 1;
            else
                left = mid + 1;
        }

        return left;
    }
    
    private static int CompareStartTime(ILevelObject x, ILevelObject y)
    {
        return x.StartTime.CompareTo(y.StartTime);
    }
    
    private static int CompareKillTime(ILevelObject x, ILevelObject y)
    {
        return x.KillTime.CompareTo(y.KillTime);
    }
}