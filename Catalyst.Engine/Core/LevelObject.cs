namespace Catalyst.Engine.Core;

/// <summary>
/// Represents a level object.
/// </summary>
public abstract class LevelObject : IDisposable
{
    public abstract float StartTime { get; }
    public abstract float KillTime { get; }
    public abstract bool Active { get; set; }
    
    /// <param name="time">Relative to StartTime.</param>
    public abstract void Update(float time);
    
    public abstract void Dispose();
}