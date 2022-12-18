using UnityEngine;

namespace Catalyst.Patch;

public abstract class ResourcePatcher
{
    public virtual bool ShouldCache => true;
    
    public abstract Object Patch(Object asset);
}