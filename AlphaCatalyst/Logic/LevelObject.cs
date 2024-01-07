using System.Collections.Generic;
using Catalyst.Engine.Core;
using Catalyst.Engine.Core.Animation;
using Catalyst.Logic.Visual;
using UnityEngine;

namespace Catalyst.Logic;

public class LevelObject : ILevelObject
{
    public float StartTime { get; }
    public float KillTime { get; }

    private readonly Sequence<Color> colorSequence;
    private readonly float depth;
    private readonly List<LevelParentObject> parentObjects;
    private readonly VisualObject visualObject;

    public LevelObject(float startTime, float killTime, Sequence<Color> colorSequence, float depth, List<LevelParentObject> parentObjects, VisualObject visualObject)
    {
        StartTime = startTime;
        KillTime = killTime;
        this.colorSequence = colorSequence;
        this.depth = depth;
        this.parentObjects = parentObjects;
        this.visualObject = visualObject;
    }
    
    public void EnterLevel(bool active)
    {
        parentObjects[parentObjects.Count - 1].GameObject.SetActive(active);
    }

    public void UpdateTime(float time)
    {
        // Set visual object color
        Color color = colorSequence.Interpolate(time - StartTime);
        visualObject.SetColor(color);
        
        // Update parents
        float positionOffset = 0.0f;
        float scaleOffset = 0.0f;
        float rotationOffset = 0.0f;

        bool animatePosition = true;
        bool animateScale = true;
        bool animateRotation = true;

        foreach (LevelParentObject parentObject in parentObjects)
        {
            // If last parent is position parented, animate position
            if (animatePosition)
            {
                Vector2 value = parentObject.PositionSequence.Interpolate(time - parentObject.TimeOffset - positionOffset);
                parentObject.Transform.localPosition = new Vector3(value.X, value.Y, depth * 0.0005f);
            }
            
            // If last parent is scale parented, animate scale
            if (animateScale)
            {
                Vector2 value = parentObject.ScaleSequence.Interpolate(time - parentObject.TimeOffset - scaleOffset);
                parentObject.Transform.localScale = new Vector3(value.X, value.Y, 1.0f);
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
}