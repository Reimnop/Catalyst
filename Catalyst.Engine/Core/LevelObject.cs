namespace Catalyst.Engine.Core;

/// <summary>
/// Represents a level object.
/// </summary>
public abstract class LevelObject : IDisposable
{
    public abstract float StartTime { get; }
    public abstract float KillTime { get; }
    public abstract bool Active { get; set; }
    public abstract void Update(float time); // time param is relative to StartTime
    public abstract void Dispose();
}