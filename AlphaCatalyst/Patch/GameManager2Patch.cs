using Catalyst.Util;
using HarmonyLib;

namespace Catalyst.Patch;

public delegate void LevelTickEventHandler();

[HarmonyPatch(typeof(GameManager2))]
public class GameManager2Patch
{
    public static event LevelTickEventHandler LevelTick;

    [HarmonyPatch(nameof(GameManager2.Update))]
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