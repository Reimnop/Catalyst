using System.Collections.Generic;
using System.Linq;
using Catalyst.Animation;
using UnityEngine;

using GameData = DataManager.GameData;

namespace Catalyst.Logic;

public class Level
{
    private readonly List<LevelObject> levelObjects;

    private readonly HashSet<LevelObject> aliveObjects;

    private readonly List<LevelObject> objectActivateList;
    private readonly List<LevelObject> objectDeactivateList;
    
    private int activationIndex = 0;
    private int deactivationIndex = 0;

    private float lastTime = 0.0f;

    public Level(GameData gameData)
    {
        // Convert GameData to LevelObjects
        GameDataLevelObjectsConverter converter = new GameDataLevelObjectsConverter(gameData);
        levelObjects = converter.ToLevelObjects();
        
        // Activate list: sort by start time
        // For object activation in order
        objectActivateList = levelObjects.ToList();
        objectActivateList.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));
        
        // Deactivate list: sort by kill time
        // For object deactivation in order (happens after activation)
        objectDeactivateList = levelObjects.ToList();
        objectDeactivateList.Sort((x, y) => x.KillTime.CompareTo(y.KillTime));
        
        // Keep a list of alive objects
        aliveObjects = new HashSet<LevelObject>();
        
        CatalystBase.LogInfo($"Loaded {levelObjects.Count} objects (original: {gameData.beatmapObjects.Count})");
    }

    public void Update(float time)
    {
        // Update object activation status
        if (time >= lastTime)
        {
            UpdateObjectsForward(time);
        }
        else
        {
            UpdateObjectsBackward(time);
        }

        lastTime = time;
        
        // Update individual objects
        foreach (LevelObject levelObject in aliveObjects)
        {
            UpdateLevelObject(levelObject, time);
        }
    }

    private void UpdateLevelObject(LevelObject levelObject, float time)
    {
        // Set visual object color
        Sequence<Color> colorSequence = levelObject.ColorSequence;
        Color color = colorSequence.Interpolate(time - levelObject.StartTime);
        levelObject.VisualObject.SetColor(color);
        
        // Update parents
        float positionOffset = 0.0f;
        float scaleOffset = 0.0f;
        float rotationOffset = 0.0f;

        bool animatePosition = true;
        bool animateScale = true;
        bool animateRotation = true;

        foreach (LevelParentObject parentObject in levelObject.ParentObjects)
        {
            // If last parent is position parented, animate position
            if (animatePosition)
            {
                Vector2 value = parentObject.PositionSequence.Interpolate(time - parentObject.TimeOffset - positionOffset);
                parentObject.Transform.localPosition = new Vector3(value.x, value.y, levelObject.Depth * 0.0005f);
            }
            
            // If last parent is scale parented, animate scale
            if (animateScale)
            {
                Vector2 value = parentObject.ScaleSequence.Interpolate(time - parentObject.TimeOffset - scaleOffset);
                parentObject.Transform.localScale = new Vector3(value.x, value.y, 1.0f);
            }
            
            // If last parent is rotation parented, animate rotation
            if (animateRotation)
            {
                parentObject.Transform.localRotation = Quaternion.AngleAxis(
                    parentObject.RotationSequence.Interpolate(time - parentObject.TimeOffset - rotationOffset),
                    Vector3.forward);
            }
            
            // Cache parent values to use for next parent
            positionOffset = parentObject.ParentOffsetPosition;
            scaleOffset = parentObject.ParentOffsetScale;
            rotationOffset = parentObject.ParentOffsetRotation;

            animatePosition = parentObject.ParentAnimatePosition;
            animateScale = parentObject.ParentAnimateScale;
            animateRotation = parentObject.ParentAnimateRotation;
        }
    }

    private void UpdateObjectsForward(float time)
    {
        while (activationIndex < objectActivateList.Count && time >= objectActivateList[activationIndex].StartTime)
        {
            objectActivateList[activationIndex].TopParentObject.GameObject.SetActive(true);
            aliveObjects.Add(objectActivateList[activationIndex]);
            activationIndex++;
        }

        while (deactivationIndex < objectDeactivateList.Count && time >= objectDeactivateList[deactivationIndex].KillTime)
        {
            objectDeactivateList[deactivationIndex].TopParentObject.GameObject.SetActive(false);
            aliveObjects.Remove(objectDeactivateList[deactivationIndex]);
            deactivationIndex++;
        }
    }

    private void UpdateObjectsBackward(float time)
    {
        while (deactivationIndex - 1 >= 0 && time < objectDeactivateList[deactivationIndex - 1].KillTime)
        {
            objectDeactivateList[deactivationIndex - 1].TopParentObject.GameObject.SetActive(true);
            aliveObjects.Add(objectDeactivateList[deactivationIndex - 1]);
            deactivationIndex--;
        }

        while (activationIndex - 1 >= 0 && time < objectActivateList[activationIndex - 1].StartTime)
        {
            objectActivateList[activationIndex - 1].TopParentObject.GameObject.SetActive(false);
            aliveObjects.Remove(objectActivateList[activationIndex - 1]);
            activationIndex--;
        }
    }
}