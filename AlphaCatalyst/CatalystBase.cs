using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Catalyst.Logic;
using Catalyst.Patch;
using HarmonyLib;
using UnityEngine;

namespace Catalyst;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInProcess("Project Arrhythmia.exe")]
public class CatalystBase : BasePlugin
{
    // I have become the very thing I swore to destroy
    public static CatalystBase Instance;
    
    public ManualLogSource Logger { get; private set; }

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
    
    public override void Load()
    {
        Instance = this;
        
        Logger = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_NAME);
        
        LogInfo("Patching Project Arrhythmia");
        
        harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
        
        LogInfo("Attaching hooks");
        
        // TODO: Fix this, it's broken
        // GameManagerPatch.LevelStart += OnLevelStart;
        
        GameManagerPatch.LevelEnd += OnLevelEnd;
        ObjectManagerPatch.LevelTick += OnLevelTick;
        
        LogInfo($"{PluginInfo.PLUGIN_NAME} is initialized and ready!");
    }

    private void OnLevelStart()
    {
        LogInfo("Loading level");
        
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
        // Hack level start
        // TODO: Dirty hack, fix this
        if (levelProcessor == null && DataManager.inst.gameData.beatmapObjects.Count > 0)
        {
            OnLevelStart();
            return;
        }
        
        var currentAudioTime = AudioManager.inst.CurrentAudioSource.time;
        var smoothedTime = Mathf.SmoothDamp(previousAudioTime, currentAudioTime, ref audioTimeVelocity, 1.0f / 50.0f);
        levelProcessor?.Update(smoothedTime);
        previousAudioTime = smoothedTime;
    }
}