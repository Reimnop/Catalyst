using BepInEx;
using Catalyst.Common;
using Catalyst.Logic;
using Catalyst.Patch;
using HarmonyLib;
using UnityEngine;

namespace Catalyst;

[BepInPlugin(ModMetadata.Guid, ModMetadata.Name, ModMetadata.Version)]
[BepInProcess("Project Arrhythmia.exe")]
public class CatalystBase : BaseUnityPlugin
{
    // I have become the very thing I swore to destroy
    public static CatalystBase Instance;

    private Harmony harmony;
    private LevelProcessor levelProcessor;
    
    private float previousAudioTime;
    private float audioTimeVelocity;
    
    public static void LogInfo(object msg)
    {
        Instance.Logger.LogInfo(msg);
    }
    
    public static void LogWarning(object msg)
    {
        Instance.Logger.LogWarning(msg);
    }
    
    public static void LogError(object msg)
    {
        Instance.Logger.LogError(msg);
    }

    private void Awake()
    {
        Instance = this;
        
        LogInfo("Patching Project Arrhythmia");
        
        harmony = new Harmony(ModMetadata.Guid);
        harmony.PatchAll();
        
        LogInfo("Attaching hooks");

        GameManagerPatch.LevelStart += OnLevelStart;
        GameManagerPatch.LevelEnd += OnLevelEnd;
        ObjectManagerPatch.LevelTick += OnLevelTick;

        LogInfo($"{ModMetadata.Name} is initialized and ready!");
    }

    private void OnLevelStart()
    {
        LogInfo("Loading level");
        
        previousAudioTime = 0.0f;
        audioTimeVelocity = 0.0f;
        levelProcessor = new LevelProcessor(DataManager.inst.gameData);
    }
    
    private void OnLevelEnd()
    {
        LogInfo("Cleaning up level");
        
        levelProcessor.Dispose();
        levelProcessor = null;
    }

    private void OnLevelTick()
    {
        var currentAudioTime = AudioManager.inst.CurrentAudioSource.time;
        var smoothedTime = Mathf.SmoothDamp(previousAudioTime, currentAudioTime, ref audioTimeVelocity, 1.0f / 50.0f);
        levelProcessor?.Update(smoothedTime);
        previousAudioTime = smoothedTime;
    }
}