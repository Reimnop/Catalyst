namespace Catalyst.Engine.Core;

/// <summary>
/// Represents a level object.
/// </summary>
public interface ILevelObject
{
    float StartTime { get; }
    float KillTime { get; }
    
    /// <summary>
    /// Called when the object enters the level. (spawned)
    /// </summary>
    void EnterLevel();
    
    /// <summary>
    /// Called when the object exits the level. (killed)
    /// </summary>
    void ExitLevel();

    /// <param name="time">Seconds since the level has started.</param>
    void UpdateTime(float time);
}