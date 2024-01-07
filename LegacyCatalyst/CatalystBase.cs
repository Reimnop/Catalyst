﻿using BepInEx;
using Catalyst.Logic;
using Catalyst.Patch;
using HarmonyLib;
using UnityEngine;

namespace Catalyst;

[BepInPlugin(Guid, Name, Version)]
[BepInProcess("Project Arrhythmia.exe")]
public class CatalystBase : BaseUnityPlugin
{
    // I have become the very thing I swore to destroy
    public static CatalystBase Instance;
    
    public const string Guid = "me.reimnop.catalyst";
    public const string Name = "Catalyst";
#if DEBUG
    public const string Version = "2.2.0 [DEBUG]";
#else
    public const string Version = "2.2.0";
#endif

    public const string Description = "Next-generation performance mod for Project Arrhythmia - Successor of Potassium";

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
        
        harmony = new Harmony(Guid);
        harmony.PatchAll();
        
        LogInfo("Attaching hooks");

        GameManagerPatch.LevelStart += OnLevelStart;
        GameManagerPatch.LevelEnd += OnLevelEnd;
        ObjectManagerPatch.LevelTick += OnLevelTick;

        LogInfo($"{Name} is initialized and ready!");
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
        var smoothedTime = Mathf.SmoothDamp(previousAudioTime, currentAudioTime, ref audioTimeVelocity, Time.deltaTime);
        levelProcessor?.Update(smoothedTime);
        previousAudioTime = currentAudioTime;
    }
}