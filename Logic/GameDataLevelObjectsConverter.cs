using System;
using System.Collections.Generic;
using System.Linq;
using Catalyst.Animation;
using Catalyst.Logic.Visual;
using UnityEngine;

using GameData = DataManager.GameData;
using BeatmapObject = DataManager.GameData.BeatmapObject;
using EventKeyframe = DataManager.GameData.EventKeyframe;
using Object = UnityEngine.Object;
using ObjectType = DataManager.GameData.BeatmapObject.ObjectType;

namespace Catalyst.Logic;

// WARNING: This class has side effects and will instantiate GameObjects
// Converts GameData to LevelObjects to be used by the mod
public class GameDataLevelObjectsConverter
{
    private class CachedSequences
    {
        public Sequence<Vector2, Vector2> PositionSequence { get; set; }
        public Sequence<Vector2, Vector2> ScaleSequence { get; set; }
        public Sequence<float, float> RotationSequence { get; set; }
        public Sequence<int, Color> ColorSequence { get; set; }
    }

    private readonly Dictionary<string, CachedSequences> cachedSequences;
    private readonly Dictionary<string, BeatmapObject> beatmapObjects;

    private readonly GameData gameData;

    public GameDataLevelObjectsConverter(GameData gameData)
    {
        this.gameData = gameData;

        beatmapObjects = new Dictionary<string, BeatmapObject>();
        
        foreach (BeatmapObject beatmapObject in gameData.beatmapObjects)
        {
            if (beatmapObjects.ContainsKey(beatmapObject.id))
            {
                return;
            }
            
            beatmapObjects.Add(beatmapObject.id, beatmapObject);
        }
        
        // Cache sequences
        cachedSequences = new Dictionary<string, CachedSequences>();
        
        foreach (BeatmapObject beatmapObject in beatmapObjects.Values)
        {
            CachedSequences collection = new CachedSequences()
            {
                PositionSequence = GetVector2Sequence(beatmapObject.events[0], new Keyframe<Vector2>(0.0f, Vector2.zero, Easing.Linear)),
                ScaleSequence = GetVector2Sequence(beatmapObject.events[1], new Keyframe<Vector2>(0.0f, Vector2.one, Easing.Linear)),
                RotationSequence = GetFloatSequence(beatmapObject.events[2], new Keyframe<float>(0.0f, 0.0f, Easing.Linear), true)
            };
            
            // Empty objects don't need a color sequence, so it is not cached
            if (beatmapObject.objectType != ObjectType.Empty)
            {
                collection.ColorSequence = GetColorSequence(beatmapObject.events[3], new Keyframe<int>(0.0f, 0, Easing.Linear));
            }
            
            cachedSequences.Add(beatmapObject.id, collection);
        }
    }

    public List<LevelObject> ToLevelObjects()
    {
        List<LevelObject> levelObjects = new List<LevelObject>();

        foreach (BeatmapObject beatmapObject in gameData.beatmapObjects)
        {
            if (beatmapObject.objectType == ObjectType.Empty)
            {
                continue;
            }
            
            levelObjects.Add(ToLevelObject(beatmapObject));
        }
        
        return levelObjects;
    }

    private LevelObject ToLevelObject(BeatmapObject beatmapObject)
    {
        List<LevelParentObject> parentObjects = new List<LevelParentObject>();

        GameObject parent = null;
        if (!string.IsNullOrEmpty(beatmapObject.parent) && beatmapObjects.ContainsKey(beatmapObject.parent))
        {
            parent = InitParentChain(beatmapObjects[beatmapObject.parent], parentObjects);
        }
        
        GameObject baseObject = Object.Instantiate(ObjectManager.inst.objectPrefabs[beatmapObject.shape].options[beatmapObject.shapeOption], parent == null ? null : parent.transform);
        parentObjects.Insert(0, InitLevelParentObject(beatmapObject, baseObject));

        LevelParentObject topParentObject = parentObjects[parentObjects.Count - 1];
        topParentObject.Transform.SetParent(ObjectManager.inst.objectParent.transform);
        topParentObject.GameObject.SetActive(false);
        
        GameObject visualObject = baseObject.transform.GetChild(0).gameObject;
        visualObject.transform.localPosition = new Vector3(beatmapObject.origin.x, beatmapObject.origin.y, beatmapObject.Depth * 0.1f);
        
        baseObject.SetActive(true);
        visualObject.SetActive(true);
        
        // Init visual object wrapper
        float opacity = beatmapObject.objectType == ObjectType.Helper ? 0.35f : 1.0f;
        bool hasCollider = beatmapObject.objectType == ObjectType.Helper ||
                           beatmapObject.objectType == ObjectType.Decoration;
        
        // 4 = text object
        VisualObject visual = beatmapObject.shape == 4
            ? new TextObject(visualObject, opacity, beatmapObject.text)
            : new SolidObject(visualObject, opacity, hasCollider);

        return new LevelObject()
        {
            StartTime = beatmapObject.StartTime,
            KillTime = beatmapObject.StartTime + beatmapObject.GetObjectLifeLength(0.0f, true),
            
            Depth = beatmapObject.Depth,
            
            ColorSequence = cachedSequences[beatmapObject.id].ColorSequence,
            
            ParentObjects = parentObjects,
            VisualObject = visual
        };
    }

    private GameObject InitParentChain(BeatmapObject beatmapObject, List<LevelParentObject> parentObjects)
    {
        GameObject gameObject = new GameObject(beatmapObject.name);
        parentObjects.Add(InitLevelParentObject(beatmapObject, gameObject));
        
        // Has parent - init parent (recursive)
        if (!string.IsNullOrEmpty(beatmapObject.parent) && beatmapObjects.ContainsKey(beatmapObject.parent))
        {
            GameObject parentObject = InitParentChain(beatmapObjects[beatmapObject.parent], parentObjects);
            gameObject.transform.SetParent(parentObject.transform);
        }

        return gameObject;
    }

    private LevelParentObject InitLevelParentObject(BeatmapObject beatmapObject, GameObject gameObject)
    {
        CachedSequences cachedSequences = this.cachedSequences[beatmapObject.id];
        return new LevelParentObject()
        {
            PositionSequence = cachedSequences.PositionSequence.Copy(),
            ScaleSequence = cachedSequences.ScaleSequence.Copy(),
            RotationSequence = cachedSequences.RotationSequence.Copy(),

            TimeOffset = beatmapObject.StartTime,

            ParentAnimatePosition = beatmapObject.GetParentType(0),
            ParentAnimateScale = beatmapObject.GetParentType(1),
            ParentAnimateRotation = beatmapObject.GetParentType(2),

            ParentOffsetPosition = beatmapObject.getParentOffset(0),
            ParentOffsetScale = beatmapObject.getParentOffset(1),
            ParentOffsetRotation = beatmapObject.getParentOffset(2),

            GameObject = gameObject,
            Transform = gameObject.transform
        };
    }

    private Sequence<Vector2, Vector2> GetVector2Sequence(List<EventKeyframe> eventKeyframes, Keyframe<Vector2> defaultKeyframe, bool relative = false)
    {
        List<Keyframe<Vector2>> keyframes = new List<Keyframe<Vector2>>(eventKeyframes.Count);

        Vector2 currentValue = Vector2.zero;
        foreach (EventKeyframe eventKeyframe in eventKeyframes)
        {
            Vector2 value = new Vector2(eventKeyframe.eventValues[0], eventKeyframe.eventValues[1]);
            if (eventKeyframe.random != 0)
            {
                ObjectManager.inst.RandomVector2Parser(eventKeyframe, out value.x, out value.y);
            }

            currentValue = relative ? currentValue + value : value;
            
            keyframes.Add(new Keyframe<Vector2>(eventKeyframe.eventTime, currentValue, Ease.EaseStringLookup[eventKeyframe.curveType.Name]));
        }
        
        // If there is no keyframe, add default
        if (keyframes.Count == 0)
        {
            keyframes.Add(defaultKeyframe);
        } 
        
        return new Sequence<Vector2, Vector2>(keyframes, Interpolators.Vector2Interpolator);
    }

    private Sequence<float, float> GetFloatSequence(List<EventKeyframe> eventKeyframes, Keyframe<float> defaultKeyframe, bool relative = false)
    {
        List<Keyframe<float>> keyframes = new List<Keyframe<float>>(eventKeyframes.Count);

        float currentValue = 0.0f;
        foreach (EventKeyframe eventKeyframe in eventKeyframes)
        {
            float value = eventKeyframe.eventValues[0];
            if (eventKeyframe.random != 0)
            {
                ObjectManager.inst.RandomFloatParser(eventKeyframe, out value);
            }

            currentValue = relative ? currentValue + value : value;
            
            keyframes.Add(new Keyframe<float>(eventKeyframe.eventTime, currentValue, Ease.EaseStringLookup[eventKeyframe.curveType.Name]));
        }
        
        // If there is no keyframe, add default
        if (keyframes.Count == 0)
        {
            keyframes.Add(defaultKeyframe);
        } 

        return new Sequence<float, float>(keyframes, Interpolators.FloatInterpolator);
    }
    
    private Sequence<int, Color> GetColorSequence(List<EventKeyframe> eventKeyframes, Keyframe<int> defaultKeyframe)
    {
        List<Keyframe<int>> keyframes = new List<Keyframe<int>>(eventKeyframes.Count);

        foreach (EventKeyframe eventKeyframe in eventKeyframes)
        {
            int value = (int) eventKeyframe.eventValues[0];

            keyframes.Add(new Keyframe<int>(eventKeyframe.eventTime, value, Ease.EaseStringLookup[eventKeyframe.curveType.Name]));
        }
        
        // If there is no keyframe, add default
        if (keyframes.Count == 0)
        {
            keyframes.Add(defaultKeyframe);
        } 
        
        return new Sequence<int, Color>(keyframes, Interpolators.ThemeInterpolator);
    }
}