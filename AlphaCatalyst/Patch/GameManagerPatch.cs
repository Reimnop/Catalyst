using Catalyst.Util;
using HarmonyLib;

namespace Catalyst.Patch;

public delegate void LevelEventHandler();

[HarmonyPatch(typeof(GameManager))]
public class GameManagerPatch
{
    public static event LevelEventHandler LevelStart;
    public static event LevelEventHandler LevelEnd;
    
    [HarmonyPatch(nameof(GameManager.PlayLevel))]
    [HarmonyPostfix]
    public static void PlayLevelPostfix()
    {
        if (Utils.IsInEditor())
        {
            return;
        }
        
        LevelStart?.Invoke();
    }
    
    [HarmonyPatch(nameof(GameManager.ExitLevel))]
    [HarmonyPostfix]
    public static void ExitLevelPostfix()
    {
        if (Utils.IsInEditor())
        {
            return;
        }
        
        LevelEnd?.Invoke();
    }
}