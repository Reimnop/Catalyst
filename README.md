![Logo](logo.png)

# Catalyst

 Next-generation performance mod for Project Arrhythmia - Successor of Potassium
 
 Made for the development & legacy branch
 
 **Does not support the Editor!**
 
 ## Usage
 
### For default branch

> [As of December 28th, 2022, this mod no longer works on `editor-alpha` branch due to BepInEx not working properly on it. Unfortunately, there is nothing that can be done on my side and requires BepInEx to fix it on their side.](https://github.com/BepInEx/BepInEx/issues/536)

- Download [BepInEx 6 x64 IL2CPP](https://github.com/BepInEx/BepInEx/releases/tag/v6.0.0-pre.1) (The file that says `BepInEx_UnityIL2CPP_x64_6.0.0-pre.1.zip`)
- To open `Project Arrhythmia Path`, go to `Project Arrhythmia` in your Steam library, click the `Settings` icon, go to `Local flies` and click `Browse`
- Extract the downloaded zip file into `<Project Arrhythmia Path>` using [WinRAR](https://www.rarlab.com/) or [7-zip](https://www.7-zip.org/)
- Launch Project Arrhythmia
- If you have done everything correctly, there should be a `plugins` folder in `<Project Arrhythmia Path>/BepInEx`
- Close Project Arrhythmia
- Drop the [AlphaCatalyst DLL](https://github.com/Reimnop/Catalyst/releases) and the [Catalyst.Engine DLL](https://github.com/Reimnop/Catalyst/releases) into the `plugins` folder
- The mod is now installed, you can now enjoy your boosted framerate

### For legacy branch

- Download [BepInEx 5 x64](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.21) (The file that says `BepInEx_x64_5.4.21.0.zip`)
- To open `Project Arrhythmia Path`, go to `Project Arrhythmia` in your Steam library, click the `Settings` icon, go to `Local flies` and click `Browse`
- Extract the downloaded zip file into `<Project Arrhythmia Path>` using [WinRAR](https://www.rarlab.com/) or [7-zip](https://www.7-zip.org/)
- Launch Project Arrhythmia
- If you have done everything correctly, there should be a `plugins` folder in `<Project Arrhythmia Path>/BepInEx`
- Close Project Arrhythmia
- Drop the [LegacyCatalyst DLL](https://github.com/Reimnop/Catalyst/releases) and the [Catalyst.Engine DLL](https://github.com/Reimnop/Catalyst/releases) into the `plugins` folder
- The mod is now installed, you can now enjoy your boosted framerate
 
 ### For development branch
 
- Download the core libraries from [BepInEx](https://unity.bepinex.dev/corlibs/2019.3.12.zip)
- Download the Unity libraries also from [BepInEx](https://unity.bepinex.dev/libraries/2019.3.12.zip)

Those steps are required because the development branch DLLs are stripped so we need the original DLLs to unstrip them (otherwise BepInEx won't load)

- Extract everything inside the core libraries zip file from BepInEx into `<Project Arrhythmia Path>/Project Arrhythmia_Data/Managed`, choose Replace All
- Extract everything inside the Unity libraries zip file from BepInEx into `<Project Arrhythmia Path>/Project Arrhythmia_Data/Managed`, also choose Replace All
- Download [BepInEx 5 x64](https://github.com/BepInEx/BepInEx/releases)
- Extract the downloaded zip file into `<Project Arrhythmia Path>`
- Launch Project Arrhythmia
- If you have done everything correctly, there should be a `plugins` folder in `<Project Arrhythmia Path>/BepInEx`
- Close Project Arrhythmia
- Drop the [DevelopmentCatalyst DLL](https://github.com/Reimnop/Catalyst/releases) and the [Catalyst.Engine DLL](https://github.com/Reimnop/Catalyst/releases) into the `plugins` folder
- The mod is now installed, you can now enjoy your boosted framerate
