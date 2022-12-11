using UnityEngine;

namespace Catalyst.Logic.Visual;

public abstract class VisualObject
{
    public abstract GameObject GameObject { get; }

    public abstract void SetColor(Color color);
}