using System;

namespace Catalyst.Animation;

public class NoKeyframeException : Exception
{
    public NoKeyframeException(string message) : base(message)
    {
    }
}