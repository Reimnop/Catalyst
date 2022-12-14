using System;

namespace Catalyst.Engine.Core.Animation;

public class NoKeyframeException : Exception
{
    public NoKeyframeException(string message) : base(message)
    {
    }
}