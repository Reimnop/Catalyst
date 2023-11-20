using System;
using System.Collections.Generic;
using Catalyst.Engine.Core;

namespace Catalyst.Engine.Data;

/// <summary>
/// Non-generic view for <see cref="Level{T}"/>.
/// </summary>
public interface ILevelView : IEnumerable<ILevelObject>
{
    event EventHandler<ILevelObject>? ObjectInserted;
    event EventHandler<ILevelObject>? ObjectRemoved;
}