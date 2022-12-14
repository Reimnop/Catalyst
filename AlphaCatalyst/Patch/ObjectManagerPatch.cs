using Catalyst.Util;
using HarmonyLib;

namespace Catalyst.Patch;

public delegate void LevelTickEventHandler();

[HarmonyPatch(typeof(ObjectManager))]
public class ObjectManagerPatch
{
    public static event LevelTickEventHandler LevelTick;

    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    public static bool UpdatePrefix()
    {
        if (Utils.IsInEditor())
        {
            return true;
        }
        
        LevelTick?.Invoke();
        return false;
    }
}