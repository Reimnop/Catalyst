using System.Collections.Generic;
using Catalyst.Animation;
using Catalyst.Logic.Visual;
using UnityEngine;

namespace Catalyst.Logic;

public class LevelObject
{
    public float StartTime { get; set; }
    public float KillTime { get; set; }
    
    public float Depth { get; set; }

    public Sequence<int, Color> ColorSequence { get; set; }

    // First element: bottom of the chain
    // Last element: top of the chain
    public List<LevelParentObject> ParentObjects { get; set; }
    public VisualObject VisualObject { get; set; }

    public LevelParentObject TopParentObject => ParentObjects[ParentObjects.Count - 1];
}