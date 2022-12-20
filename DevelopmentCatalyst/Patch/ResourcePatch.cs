using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Catalyst.Patch;

public static class ResourcePatch
{
    private static readonly Dictionary<string, ResourcePatcher> patchers = new Dictionary<string, ResourcePatcher>();
    private static readonly Dictionary<string, Object> cache = new Dictionary<string, Object>();

    // The attributes don't work for some reason, so we have to do it manually
    public static void Init(Harmony harmony)
    {
        // Patch allAssets getter
        MethodInfo allAssetsMethod = typeof(AssetBundleRequest).GetProperty(nameof(AssetBundleRequest.allAssets), BindingFlags.Instance | BindingFlags.Public).GetGetMethod();
        MethodInfo allAssetsPostfixMethod = typeof(ResourcePatch).GetMethod(nameof(AllAssetsPostfix), BindingFlags.Static | BindingFlags.NonPublic);
        harmony.Patch(allAssetsMethod, null, new HarmonyMethod(allAssetsPostfixMethod));
        
        // Patch asset getter
        MethodInfo assetMethod = typeof(AssetBundleRequest).GetProperty(nameof(AssetBundleRequest.asset), BindingFlags.Instance | BindingFlags.Public).GetGetMethod();
        MethodInfo assetPostfixMethod = typeof(ResourcePatch).GetMethod(nameof(AssetPostfix), BindingFlags.Static | BindingFlags.NonPublic);
        harmony.Patch(assetMethod, null, new HarmonyMethod(assetPostfixMethod));
    }
    
    public static void RegisterPatcher(string assetName, ResourcePatcher patcher)
    {
        patchers.Add(assetName, patcher);
    }

    private static void AllAssetsPostfix(ref Object[] __result)
    {
        __result = __result
            .Select(o => TryPatchAsset(o, out Object patchedAsset) ? patchedAsset : o)
            .ToArray();
    }
    
    private static void AssetPostfix(ref Object __result)
    {
        if (TryPatchAsset(__result, out Object redirect))
        {
            __result = redirect;
        }
    }
    
    private static bool TryPatchAsset(Object original, out Object asset)
    {
        if (cache.TryGetValue(original.name, out asset))
        {
            return true;
        }
        
        if (patchers.TryGetValue(original.name, out ResourcePatcher patcher))
        {
            asset = patcher.Patch(original);

            if (patcher.ShouldCache)
            {
                cache.Add(original.name, asset);
            }
            return true;
        }
        
        return false;
    }
}