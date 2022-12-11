using Catalyst.Animation;
using UnityEngine;

namespace Catalyst.Logic;

public class LevelParentObject
{
    public Sequence<Vector2, Vector2> PositionSequence { get; set; }
    public Sequence<Vector2, Vector2> ScaleSequence { get; set; }
    public Sequence<float, float> RotationSequence { get; set; }

    public float TimeOffset { get; set; }

    public bool ParentAnimatePosition { get; set; }
    public bool ParentAnimateScale { get; set; }
    public bool ParentAnimateRotation { get; set; }

    public float ParentOffsetPosition { get; set; }
    public float ParentOffsetScale { get; set; }
    public float ParentOffsetRotation { get; set; }

    public GameObject GameObject { get; set; }
    public Transform Transform { get; set; }
}