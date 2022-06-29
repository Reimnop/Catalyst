# Catalyst

(thank you Rin Chiropteran for the name!)

 Next-generation performance mod for Project Arrhythmia - Successor of Potassium
 
 Made for the development & legacy branch
 
 ## Usage
 
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
- Drop the [Catalyst DLL](https://github.com/Reimnop/Catalyst/releases) into the `plugins` folder
- The mod is now installed, you can now enjoy your boosted framerate

## Why all of this over Potassium

This mod was made to be a "cleaner code" variation of Potassium, as well as being faster. It is for research purposes, as the original Potassium codebase is a mess and a nightmare to deal with as another person trying to understand Potassium's code

If you're just a casual player, this mod is probably not for you (unless you're extremely desparate for the absolute maximum FPS)

If you still want to use Catalyst, check out Legacy Catalyst below

### For legacy branch

- Download [BepInEx 5 x64](https://github.com/BepInEx/BepInEx/releases)
- Extract the downloaded zip file into `<Project Arrhythmia Path>`
- Launch Project Arrhythmia
- If you have done everything correctly, there should be a `plugins` folder in `<Project Arrhythmia Path>/BepInEx`
- Close Project Arrhythmia
- Drop the [LegacyCatalyst DLL](https://github.com/Reimnop/Catalyst/releases) into the `plugins` folder
- The mod is now installed, you can now enjoy your boosted framerate
