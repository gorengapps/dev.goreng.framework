 # The Framework
 
The framework consists of various elements to make your Unity life a bit easier. 
If you try to fight the framework you will have a hard time. 
But extending it is quite easy.
## Documentation

- [Dependency Injection](/Documentation/Dependency%20Injection.md)
- Bootstrap (T.B.A)
- Canvas (T.B.A)
## Requirments

- Unity 2022.3.x LTS
## Installation

Add a new "Scoped Registery" to your package manager and use the following url
https://packages.goreng.dev you should be able to see The Framework package in your list.
you can use the scope `dev.goreng`
## Changelog
### 0.2.3
- Removed Monobehaviour from `NavigationService` and `DataService` please use class injection to make them available to your project
- Added IRunLoop which allow you to give non monobehaviours coroutine logic and update loops
- AbstractBootstrap now has a default `NavigationService` this was done since navigation service is essential.
- Added a `[FetchCanvas]` attribute allowing to prefetch the canvas automagically
- Added a menu `Tools/Force Service Scene` This allows you to force the Services scene to start when pressing play.

### 0.2.0
- Updated how we handle scene unloading, we will now wait until the task was completed 
- Added an Implicit Used flag on Injectable so we wont generate warnings
### 0.1.9
- Bugfix registration of Interfaced types
### 0.1.7
- Add a new way to constructor inject objects that are not prefabs
- Add Documentation on how the DI works
### 0.1.6
- Update the correct prefabs, i finally figured out how they work :)
### 0.1.5
 - Update the Readme so we have some more information on this package 
 - Fix `FetchCanvas` to also support searching child GameObject for canvases
 - Renamed Dataservice `Load` to `LoadList` since we always load a list of data
### 0.1.0
- The first public release of the package

  
