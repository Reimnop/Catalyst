using System;
using System.Collections.Generic;
using Catalyst.Engine.Data;

namespace Catalyst.Engine.Core;

/// <summary>
/// Handles object spawning and despawning.
/// </summary>
public class ObjectSpawner : IDisposable
{
    public IEnumerable<ILevelObject> ActiveObjects => activeObjects;
    private readonly HashSet<ILevelObject> activeObjects = new();

    private readonly ILevelView levelView;
    
    // Stuff
    private readonly List<ILevelObject> activateList = new();
    private readonly List<ILevelObject> deactivateList = new();
    
    private int activateIndex = 0;
    private int deactivateIndex = 0;
    private float currentTime = 0.0f;
    
    // Queues
    private readonly HashSet<ILevelObject> startTimeUpdateQueue = new();
    private readonly HashSet<ILevelObject> killTimeUpdateQueue = new();
    private readonly HashSet<ILevelObject> insertQueue = new();
    private readonly HashSet<ILevelObject> removeQueue = new();

    public ObjectSpawner(ILevelView levelView)
    {
        this.levelView = levelView;
        
        // populate activate and deactivate lists
        activateList.AddRange(levelView);
        deactivateList.AddRange(levelView);

        // sort them
        activateList.Sort(CompareStartTime);
        deactivateList.Sort(CompareKillTime);
        
        // Subscribe to events
        levelView.ObjectInserted += OnObjectInserted;
        levelView.ObjectRemoved += OnObjectRemoved;

        foreach (var levelObject in levelView)
        {
            levelObject.PropertyChanged += LevelObjectOnPropertyChanged;
        }
    }

    private void LevelObjectOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var levelObject = (ILevelObject) sender!;
        switch (e.PropertyName)
        {
            case nameof(ILevelObject.StartTime):
                // Queue object for start time update
                startTimeUpdateQueue.Add(levelObject);
                break;
            case nameof(ILevelObject.KillTime):
                // Queue object for kill time update
                killTimeUpdateQueue.Add(levelObject);
                break;
        }
    }

    private void OnObjectInserted(object sender, ILevelObject e)
    {
        // Queue object for insertion
        insertQueue.Add(e);
    }
    
    private void OnObjectRemoved(object sender, ILevelObject e)
    {
        // Queue object for removal
        removeQueue.Add(e);
    }
    
    private void UpdateLevelObjectActiveState(ILevelObject levelObject, float time)
    {
        var objectActive = activeObjects.Contains(levelObject);
        if (time >= levelObject.StartTime && time < levelObject.KillTime && !objectActive)
        {
            // Activate the object
            levelObject.EnterLevel();
            activeObjects.Add(levelObject);
        }
        else if (objectActive)
        {
            // Deactivate the object
            levelObject.ExitLevel();
            activeObjects.Remove(levelObject);
        }
    }
    
    private void ProcessStartTimeUpdateQueue(float time)
    {
        foreach (var levelObject in startTimeUpdateQueue)
        {
            UpdateLevelObjectActiveState(levelObject, time);
        }
        startTimeUpdateQueue.Clear();

        // Sort and update
        activateList.Sort(CompareStartTime);
        activateIndex = CalculateIndex(time, activateList, x => x.StartTime);
    }

    private void ProcessKillTimeUpdateQueue(float time)
    {
        foreach (var levelObject in killTimeUpdateQueue)
        {
            UpdateLevelObjectActiveState(levelObject, time);
        }
        killTimeUpdateQueue.Clear();

        // Sort and update
        deactivateList.Sort(CompareKillTime);
        deactivateIndex = CalculateIndex(time, deactivateList, x => x.KillTime);
    }

    private void ProcessInsertQueue(float time)
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
            
            // Subscribe to events
            levelObject.PropertyChanged += LevelObjectOnPropertyChanged;
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

    private void ProcessRemoveQueue(float time)
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
            
            // Unsubscribe from events
            levelObject.PropertyChanged -= LevelObjectOnPropertyChanged;
        }
            
        // Clear queue
        removeQueue.Clear();
            
        // We don't need to sort here, since we only remove objects,
        // which means that the lists are still sorted.
            
        // Recalculate indices
        activateIndex = CalculateIndex(time, activateList, x => x.StartTime);
        deactivateIndex = CalculateIndex(time, deactivateList, x => x.KillTime);
    }

    public void Update(float time)
    {
        // Pre-update queue processing
        if (startTimeUpdateQueue.Count > 0)
        {
            ProcessStartTimeUpdateQueue(time);
        }
        
        if (killTimeUpdateQueue.Count > 0)
        {
            ProcessKillTimeUpdateQueue(time);
        }
        
        if (insertQueue.Count > 0)
        {
            ProcessInsertQueue(time);
        }
        
        if (removeQueue.Count > 0)
        {
            ProcessRemoveQueue(time);
        }
        
        // Actual update logic
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

    public void Dispose()
    {
        levelView.ObjectInserted -= OnObjectInserted;
        levelView.ObjectRemoved -= OnObjectRemoved;
        
        foreach (var levelObject in levelView)
        {
            levelObject.PropertyChanged -= LevelObjectOnPropertyChanged;
        }
    }
}