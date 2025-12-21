# Scene Service
A small package to allow for loading and unloading scenes in groups.

---

## Features
- Scene group loading based on dependencies.
- Overrideable scene load and unload functionality
- Automatic bootstrap scene support
---
## Disclaimer
This system was developed as tech dev for my own projects. Due to time, it has _not_ been thoroughly tested
and assuming correctness is a dangerous game to play :).
If you run into issues or have suggestions, feel free to open an issue on GitHub... no promises, but I’ll help when I can.

## Installation
In the package manager select Install package from git url and paste the following two links. The first is a third party scene reference library which the scene service depends on and the second is the scene service:

```bash
https://github.com/starikcetin/Eflatun.SceneReference.git#4.1.1
```

```bash
https://github.com/TadDiDio/Scene-Service.git?path=Packages/com.radtad2.sceneservice#1.1.1
```

## Quick Start
1. Open the scene map settings on the toolbar from Tools -> Scene Map Settings.
2. Create and assign a scene map in the settings window. You can create a new one by right clicking in the assets folder and selecting Create -> Scene Map
3. Fill out the scene map. You may directly edit the asset but the scene map settings window is convenient because it is accessible without finding the asset in your project structure. See below for an explanation of the scene map.
4. In code, all scene requests should go through an ISceneController object. You can get a scene controller by using `SceneService.BuildSceneController()`, however, you should only ever create one of these and just distribute it to any module that needs it. Dependency injection is the intent rather than a singleton like Unity's `SceneManager` class.
5. You can now use this object to swap between scene groups defined in the scene map, add and remove extra scenes, and even inject custom managers when you need to define how scenes should be loaded like in the case of some multiplayer packages.

## Scene Map
The scene map is what controls which scenes are loaded together. There are 4 settings in each scene map. First is a name which is non-functional and simply to differentiate objects. Only the asset that is set in the Scene Map Settings window is included in builds so there is no confusion over which asset is being loaded.

### Bootstrapping
Next are the bootstrapping settings. You can enable or disable loading a bootstrap scene first using the `Load Boostrapper First` toggle. If unchecked, your build scene list / editor will take over and the `Bootstrap Scene` will not be used. If checked, the scene set in the `Bootstrap Scene` reference will be loaded before any other scene. 

It is strongly recommended that you use the bootstrap scene as you can perform async initialization work without other modules loading too early. If you chose to use this you can also use `SceneService.EditorGroup` to read which scenes were open in the editor when entering playmode to rebuild that group or, more likely, to load the scene group where the active scene corresponds to the editor active scene (see below).

### Scene Groups
Scene groups are the basic unit of scene loading and unloading. Defined as the final parameter in the scene map, each group has a name, an active scene, and a list of dependencies. The name is what you must use to load it, the active scene is the scene which will be active, and the dependencies are scenes which the active scene depends on. All scenes will be loaded additively and when loading all dependencies are guaranteed to finish loading before the active scene is loaded. Similarly, when loading a group, the current active group will unload the active scene first, then all dependencies.

## Generic Managers
In some cases, such as many multiplayer libraries, you may need custom control over how scenes are loaded and unloaded. You can achieve this be implementing ISceneManager and passing it as an argument to any function that you want to use it on `ISceneController`. Ideally, this object is lightweight and you can create a new one per call with specific parameters that you need. Each `ISceneManager` must implement `ISceneLoadOperation LoadSceneAsync(string scenePath)`, `ISceneLoadOperation UnloadSceneAsync(string scenePath)`, and `void SetActiveScene(string scenePath)`. The load and unload methods return an `ISceneLoadOperation` which is another interface you will need to implement for reporting progress. It should tell whether the particular scene being loaded is done and be able to report its progress.

## Observing Progress
You can read the progress of a loading scene group by subscribing to `ISceneController.OnProgress`. This is only fired for scene group loads, never for extra scene loads or for unloads.

## Logging
You can handle logs by your own custom package by calling `SceneLogger.SetImplicitLogging(false)` and subscribing to `SceneLogger.OnLog` which will send you logs directly instead of printing them.

## Notes
- When loaded scenes, it is highly recommended to use the scene path rather than the name to avoid conflicts.
- When loading a new group, if the currently active scene is the same as the next active scene it will always be reloaded regardless of the reload policy selected. This is to avoid conflicts when unloading its explicit dependencies when transitioning.