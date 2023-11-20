using System;
using System.Collections.Generic;
using System.Text;
using Catalyst.Engine.Core;
using Catalyst.Engine.Core.Animation;
using Catalyst.Engine.Core.Animation.Keyframe;
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
        public Sequence<Engine.Math.Vector2> PositionSequence { get; set; }
        public Sequence<Engine.Math.Vector2> ScaleSequence { get; set; }
        public Sequence<float> RotationSequence { get; set; }
        public Sequence<Color> ColorSequence { get; set; }
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
                PositionSequence = GetVector2Sequence(beatmapObject.events[0], new Vector2Keyframe(0.0f, Engine.Math.Vector2.Zero, Ease.Linear)),
                ScaleSequence = GetVector2Sequence(beatmapObject.events[1], new Vector2Keyframe(0.0f, Engine.Math.Vector2.One, Ease.Linear)),
                RotationSequence = GetFloatSequence(beatmapObject.events[2], new FloatKeyframe(0.0f, 0.0f, Ease.Linear), true)
            };
            
            // Empty objects don't need a color sequence, so it is not cached
            if (beatmapObject.objectType != ObjectType.Empty)
            {
                collection.ColorSequence = GetColorSequence(beatmapObject.events[3], new ColorKeyframe(0.0f, 0, Ease.Linear));
            }
            
            cachedSequences.Add(beatmapObject.id, collection);
        }
    }

    public IEnumerable<ILevelObject> ToLevelObjects()
    {
        foreach (BeatmapObject beatmapObject in gameData.beatmapObjects)
        {
            if (beatmapObject.objectType == ObjectType.Empty)
            {
                continue;
            }
            
            // Bandaid fix for invalid objects
            LevelObject levelObject = null;
            try
            {
                levelObject = ToLevelObject(beatmapObject);
            }
            catch (Exception e)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"Failed to convert object '{beatmapObject.id}' to {nameof(LevelObject)}.");
                stringBuilder.AppendLine($"Exception: {e.Message}");
                stringBuilder.AppendLine(e.StackTrace);
                
                CatalystBase.LogError(stringBuilder.ToString());
            }
            
            if (levelObject != null)
            {
                yield return levelObject;
            }
        }
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
        
        parentObjects[parentObjects.Count - 1].Transform.SetParent(ObjectManager.inst.objectParent.transform);

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

        LevelObject levelObject = new LevelObject(
            beatmapObject.StartTime,
            beatmapObject.StartTime + beatmapObject.GetObjectLifeLength(0.0f, true),
            cachedSequences[beatmapObject.id].ColorSequence,
            beatmapObject.Depth,
            parentObjects,
            visual);
        
        levelObject.EnterLevel(false);
        
        return levelObject;
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
        return new LevelParentObject
        {
            PositionSequence = cachedSequences.PositionSequence,
            ScaleSequence = cachedSequences.ScaleSequence,
            RotationSequence = cachedSequences.RotationSequence,

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

    private Sequence<Engine.Math.Vector2> GetVector2Sequence(List<EventKeyframe> eventKeyframes, Vector2Keyframe defaultKeyframe, bool relative = false)
    {
        List<IKeyframe<Engine.Math.Vector2>> keyframes = new List<IKeyframe<Engine.Math.Vector2>>(eventKeyframes.Count);

        Engine.Math.Vector2 currentValue = Engine.Math.Vector2.Zero;
        foreach (EventKeyframe eventKeyframe in eventKeyframes)
        {
            Engine.Math.Vector2 value = new Engine.Math.Vector2(eventKeyframe.eventValues[0], eventKeyframe.eventValues[1]);
            if (eventKeyframe.random != 0)
            {
                Vector2 random = ObjectManager.inst.RandomVector2Parser(eventKeyframe);
                value.X = random.x;
                value.Y = random.y;
            }

            currentValue = relative ? currentValue + value : value;
            
            keyframes.Add(new Vector2Keyframe(eventKeyframe.eventTime, value, Ease.GetEaseFunction(eventKeyframe.curveType.Name)));
        }
        
        // If there is no keyframe, add default
        if (keyframes.Count == 0)
        {
            keyframes.Add(defaultKeyframe);
        } 
        
        return new Sequence<Engine.Math.Vector2>(keyframes);
    }

    private Sequence<float> GetFloatSequence(List<EventKeyframe> eventKeyframes, FloatKeyframe defaultKeyframe, bool relative = false)
    {
        List<IKeyframe<float>> keyframes = new List<IKeyframe<float>>(eventKeyframes.Count);

        float currentValue = 0.0f;
        foreach (EventKeyframe eventKeyframe in eventKeyframes)
        {
            float value = eventKeyframe.eventValues[0];
            if (eventKeyframe.random != 0)
            {
                value = ObjectManager.inst.RandomFloatParser(eventKeyframe);
            }

            currentValue = relative ? currentValue + value : value;
            
            keyframes.Add(new FloatKeyframe(eventKeyframe.eventTime, currentValue, Ease.GetEaseFunction(eventKeyframe.curveType.Name)));
        }
        
        // If there is no keyframe, add default
        if (keyframes.Count == 0)
        {
            keyframes.Add(defaultKeyframe);
        } 

        return new Sequence<float>(keyframes);
    }
    
    private Sequence<Color> GetColorSequence(List<EventKeyframe> eventKeyframes, ColorKeyframe defaultKeyframe)
    {
        List<IKeyframe<Color>> keyframes = new List<IKeyframe<Color>>(eventKeyframes.Count);

        foreach (EventKeyframe eventKeyframe in eventKeyframes)
        {
            int value = (int) eventKeyframe.eventValues[0];

            keyframes.Add(new ColorKeyframe(eventKeyframe.eventTime, value, Ease.GetEaseFunction(eventKeyframe.curveType.Name)));
        }
        
        // If there is no keyframe, add default
        if (keyframes.Count == 0)
        {
            keyframes.Add(defaultKeyframe);
        } 
        
        return new Sequence<Color>(keyframes);
    }
}