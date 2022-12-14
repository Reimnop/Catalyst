![Logo](logo.png)

(Logo by enchart)

# Catalyst

(thank you Rin Chiropteran for the name!)

 Next-generation performance mod for Project Arrhythmia - Successor of Potassium
 
 Made for the development & legacy branch
 
 **Does not support the Editor!**
 
 ## Usage

### For legacy branch

- Download [BepInEx 5 x64](https://github.com/BepInEx/BepInEx/releases)
- Extract the downloaded zip file into `<Project Arrhythmia Path>`
- Launch Project Arrhythmia
- If you have done everything correctly, there should be a `plugins` folder in `<Project Arrhythmia Path>/BepInEx`
- Close Project Arrhythmia
- Drop the [LegacyCatalyst DLL](https://github.com/Reimnop/Catalyst/releases) and the [Catalyst.Engine DLL](https://github.com/Reimnop/Catalyst/releases) into the `plugins` folder
- The mod is now installed, you can now enjoy your boosted framerate

### For alpha (or none) branch

- Download [BepInEx 6 x64 IL2CPP](https://github.com/BepInEx/BepInEx/releases)
- Extract the downloaded zip file into `<Project Arrhythmia Path>`
- Launch Project Arrhythmia
- If you have done everything correctly, there should be a `plugins` folder in `<Project Arrhythmia Path>/BepInEx`
- Close Project Arrhythmia
- Drop the [AlphaCatalyst DLL](https://github.com/Reimnop/Catalyst/releases) and the [Catalyst.Engine DLL](https://github.com/Reimnop/Catalyst/releases) into the `plugins` folder
- The mod is now installed, you can now enjoy your boosted framerate
 
 ### For development branch
 
- Install the [Unity Editor (2019.3.12f1)](https://unity3d.com/get-unity/download/archive) 
- Download the core libraries from [BepInEx](https://unity.bepinex.dev/corlibs/2019.3.12.zip)

Those steps are required because the development branch DLLs are stripped so we need the original DLLs to unstrip them (otherwise BepInEx won't load)

- Get the original Unity DLLs in `<Editor Path>/Data/PlaybackEngines/windowsstandalonesupport/Variations/win64_nondevelopment_mono/Data/Managed` (on Windows this usually is `C:\Program Files\Unity\Hub\Editor\2019.3.12f1\Editor\Data\PlaybackEngines\windowsstandalonesupport\Variations\win64_nondevelopment_mono\Data\Managed`)
- Copy everything in the folder into `<Project Arrhythmia Path>/Project Arrhythmia_Data/Managed`, choose Replace All
- Extract everything inside the core libraries zip file from BepInEx into `<Project Arrhythmia Path>/Project Arrhythmia_Data/Managed`, also choose Replace All
- Download [BepInEx 5 x64](https://github.com/BepInEx/BepInEx/releases)
- Extract the downloaded zip file into `<Project Arrhythmia Path>`
- Launch Project Arrhythmia
- If you have done everything correctly, there should be a `plugins` folder in `<Project Arrhythmia Path>/BepInEx`
- Close Project Arrhythmia
- Drop the [DevelopmentCatalyst DLL](https://github.com/Reimnop/Catalyst/releases) and the [Catalyst.Engine DLL](https://github.com/Reimnop/Catalyst/releases) into the `plugins` folder
- The mod is now installed, you can now enjoy your boosted framerate
