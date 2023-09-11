using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using Catalyst.Logic;
using Catalyst.Patch;
using Catalyst.UI;
using HarmonyLib;

namespace Catalyst;

[BepInPlugin(Guid, Name, Version)]
[BepInProcess("Project Arrhythmia.exe")]
public class CatalystBase : BasePlugin
{
    // I have become the very thing I swore to destroy
    public static CatalystBase Instance;
    
    public const string Guid = "me.reimnop.catalyst";
    public const string Name = "Catalyst";
#if DEBUG
    public const string Version = "2.1.2 [DEBUG]";
#else
    public const string Version = "2.1.2";
#endif

    public const string Description = "Next-generation performance mod for Project Arrhythmia - Successor of Potassium";

    public ManualLogSource Logger { get; private set; }

    private Harmony harmony;
    private LevelProcessor levelProcessor;
    
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
        
        Logger = BepInEx.Logging.Logger.CreateLogSource(Name);
        
        LogInfo("Patching Project Arrhythmia");
        
        harmony = new Harmony(Guid);
        harmony.PatchAll();
        
        LogInfo("Attaching hooks");
        
        // TODO: Fix this, it's broken
        // GameManagerPatch.LevelStart += OnLevelStart;
        
        GameManagerPatch.LevelEnd += OnLevelEnd;
        ObjectManagerPatch.LevelTick += OnLevelTick;
        
        // Patch the in-game UI
        ResourcePatch.Init(harmony);
        ResourcePatch.RegisterPatcher("menu_yaml_english", new MainMenuEnglishPatcher());
        
        LogInfo($"{Name} is initialized and ready!");
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
        }
        
        levelProcessor?.Update(AudioManager.inst.CurrentAudioSource.time);
    }
}