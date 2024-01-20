using System;
using System.Text;
using Il2CppSystem.Collections.Generic;
using Catalyst.Engine.Core;
using Catalyst.Engine.Core.Animation;
using Catalyst.Engine.Core.Animation.Keyframe;
using Catalyst.Logic.Keyframe;
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
        public Sequence<Vector2> PositionSequence { get; set; }
        public Sequence<Vector2> ScaleSequence { get; set; }
        public Sequence<float> RotationSequence { get; set; }
        public Sequence<Color> ColorSequence { get; set; }
    }

    private readonly System.Collections.Generic.Dictionary<string, CachedSequences> cachedSequences;
    private readonly System.Collections.Generic.Dictionary<string, BeatmapObject> beatmapObjects;

    private readonly GameData gameData;

    public GameDataLevelObjectsConverter(GameData gameData)
    {
        this.gameData = gameData;

        beatmapObjects = new System.Collections.Generic.Dictionary<string, BeatmapObject>();
        
        foreach (var beatmapObject in gameData.beatmapObjects)
        {
            if (beatmapObjects.ContainsKey(beatmapObject.id))
            {
                return;
            }
            
            beatmapObjects.Add(beatmapObject.id, beatmapObject);
        }
        
        // Cache sequences
        cachedSequences = new System.Collections.Generic.Dictionary<string, CachedSequences>();
        
        foreach (var beatmapObject in beatmapObjects.Values)
        {
            var collection = new CachedSequences()
            {
                PositionSequence = GetVector2Sequence(beatmapObject.events[0], new Vector2Keyframe(0.0f, Vector2.zero, Ease.Linear)),
                ScaleSequence = GetVector2Sequence(beatmapObject.events[1], new Vector2Keyframe(0.0f, Vector2.one, Ease.Linear)),
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

    public System.Collections.Generic.IEnumerable<ILevelObject> ToLevelObjects()
    {
        foreach (var beatmapObject in gameData.beatmapObjects)
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
                var stringBuilder = new StringBuilder();
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
        var parentObjects = new System.Collections.Generic.List<LevelParentObject>();

        GameObject parent = null;
        if (!string.IsNullOrEmpty(beatmapObject.parent) && beatmapObjects.ContainsKey(beatmapObject.parent))
        {
            parent = InitParentChain(beatmapObjects[beatmapObject.parent], parentObjects);
        }
        
        var baseObject = (GameObject) Object.Instantiate(ObjectManager.inst.objectPrefabs[beatmapObject.shape].options[beatmapObject.shapeOption], parent == null ? null : parent.transform);
        parentObjects.Insert(0, InitLevelParentObject(beatmapObject, baseObject));
        
        parentObjects[parentObjects.Count - 1].Transform.SetParent(ObjectManager.inst.objectParent.transform);

        var visualObject = baseObject.transform.GetChild(0).gameObject;
        visualObject.transform.localPosition = new Vector3(beatmapObject.origin.x, beatmapObject.origin.y, beatmapObject.Depth * 0.1f);
        
        baseObject.SetActive(true);
        visualObject.SetActive(true);
        
        // Init visual object wrapper
        var opacity = beatmapObject.objectType == ObjectType.Helper ? 0.35f : 1.0f;
        var hasCollider = beatmapObject.objectType != ObjectType.Helper &&
                          beatmapObject.objectType != ObjectType.Decoration;
        
        // 4 = text object
        VisualObject visual = beatmapObject.shape == 4
            ? new TextObject(visualObject, opacity, beatmapObject.text)
            : new SolidObject(visualObject, opacity, hasCollider);

        var levelObject = new LevelObject(
            beatmapObject.StartTime,
            beatmapObject.StartTime + beatmapObject.GetObjectLifeLength(0.0f, true),
            cachedSequences[beatmapObject.id].ColorSequence,
            beatmapObject.Depth,
            parentObjects,
            visual);
        
        levelObject.ExitLevel();
        
        return levelObject;
    }

    private GameObject InitParentChain(BeatmapObject beatmapObject, System.Collections.Generic.List<LevelParentObject> parentObjects)
    {
        var gameObject = new GameObject(beatmapObject.name);
        parentObjects.Add(InitLevelParentObject(beatmapObject, gameObject));
        
        // Has parent - init parent (recursive)
        if (!string.IsNullOrEmpty(beatmapObject.parent) && beatmapObjects.ContainsKey(beatmapObject.parent))
        {
            var parentObject = InitParentChain(beatmapObjects[beatmapObject.parent], parentObjects);
            gameObject.transform.SetParent(parentObject.transform);
        }

        return gameObject;
    }

    private LevelParentObject InitLevelParentObject(BeatmapObject beatmapObject, GameObject gameObject)
    {
        var cachedSequences = this.cachedSequences[beatmapObject.id];
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

    private Sequence<Vector2> GetVector2Sequence(GameData.EventKeyframes eventKeyframes, Vector2Keyframe defaultKeyframe, bool relative = false)
    {
        var keyframes = new System.Collections.Generic.List<IKeyframe<Vector2>>(eventKeyframes.keyframes.Count);

        var currentValue = Vector2.zero;
        foreach (var eventKeyframe in eventKeyframes.keyframes)
        {
            var value = new Vector2(eventKeyframe.Vals[0], eventKeyframe.Vals[1]);
            if (eventKeyframe.Random != 0)
            {
                ObjectHelpers.RandomVector2Parser(eventKeyframe, out var x, out var y);
                value.x = x;
                value.y = y;
            }

            currentValue = relative ? currentValue + value : value;
            
            keyframes.Add(new Vector2Keyframe(eventKeyframe.Time, value, Ease.GetEaseFunction(eventKeyframe.curveType.Name)));
        }
        
        // If there is no keyframe, add default
        if (keyframes.Count == 0)
        {
            keyframes.Add(defaultKeyframe);
        } 
        
        return new Sequence<Vector2>(keyframes);
    }

    private Sequence<float> GetFloatSequence(GameData.EventKeyframes eventKeyframes, FloatKeyframe defaultKeyframe, bool relative = false)
    {
        var keyframes = new System.Collections.Generic.List<IKeyframe<float>>(eventKeyframes.keyframes.Count);

        var currentValue = 0.0f;
        foreach (var eventKeyframe in eventKeyframes.keyframes)
        {
            var value = eventKeyframe.Vals[0];
            if (eventKeyframe.Random != 0)
            {
                ObjectHelpers.RandomFloatParser(eventKeyframe, out value);
            }

            currentValue = relative ? currentValue + value : value;
            
            keyframes.Add(new FloatKeyframe(eventKeyframe.Time, currentValue, Ease.GetEaseFunction(eventKeyframe.curveType.Name)));
        }
        
        // If there is no keyframe, add default
        if (keyframes.Count == 0)
        {
            keyframes.Add(defaultKeyframe);
        } 

        return new Sequence<float>(keyframes);
    }
    
    private Sequence<Color> GetColorSequence(GameData.EventKeyframes eventKeyframes, ColorKeyframe defaultKeyframe)
    {
        var keyframes = new System.Collections.Generic.List<IKeyframe<Color>>(eventKeyframes.keyframes.Count);

        foreach (var eventKeyframe in eventKeyframes.keyframes)
        {
            var value = (int) eventKeyframe.Vals[0];

            keyframes.Add(new ColorKeyframe(eventKeyframe.Time, value, Ease.GetEaseFunction(eventKeyframe.curveType.Name)));
        }
        
        // If there is no keyframe, add default
        if (keyframes.Count == 0)
        {
            keyframes.Add(defaultKeyframe);
        } 
        
        return new Sequence<Color>(keyframes);
    }
}