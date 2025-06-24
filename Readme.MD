 # The Framework
 
The framework consists of various elements to make your Unity life a bit easier. 
If you try to fight the framework you will have a hard time. 
But extending it is quite easy.
## Documentation

- [Dependency Injection](/Documentation/Dependency%20Injection.md)
- [Scene Management](/Documentation/Scene%20Management.md)
## Requirements

- Unity 2022.3.x LTS
## Installation

Add a new "Scoped Registry" to your package manager and use the following url
https://packages.goreng.dev you should be able to see The Framework package in your list.
you can use the scope `dev.goreng`
## Change-log
### 1.5.1
 - Updated documentation to reflect current API
 - Renamed `ICanvas` to `IView` for better clarity
 - Renamed `[FetchCanvas]` attribute to `[FetchView]` for consistency
 - Updated method signatures to use `Awaitable` instead of `Task` for better Unity integration
 - Improved editor tooling and scene management
### 0.2.5
 - Added a custom editor window for `AsyncScene` this allows us to automate some critical configuration that was easy to miss.
	 - Automatically update the label
	 - Automatically update the scene name to reflect the filename
	 - Automatically update the addressable path on rename
 - Exposed a `IDependencyProvider` to `IView` allow you to use dependency injection in views 
 - Fixed a bug that existed in `Tools/Force Service Scene` that didn't apply the force correctly on editor restarts.
 - Update documentation
 - Add `IRef` class to allow for interface serialization.
### 0.2.4
 - Update styling
### 0.2.3
- Removed `MonoBehaviour` from `NavigationService` and `DataService` please use class injection to make them available to your project
- Added IRunLoop which allow you to give non `MonoBehaviours` coroutine logic and update loops
- AbstractBootstrap now has a default `NavigationService` this was done since navigation service is essential.
- Added a `[FetchView]` attribute allowing to prefetch the view automagically
- Added a menu `Tools/Force Service Scene` This allows you to force the Services scene to start when pressing play.

### 0.2.0
- Updated how we handle scene unloading, we will now wait until the task was completed 
- Added an Implicit Used flag on Injectable so we wont generate warnings
### 0.1.9
- Bug fix registration of Interfaced types
### 0.1.7
- Add a new way to constructor inject objects that are not prefabs
- Add Documentation on how the DI works
### 0.1.6
- Update the correct prefabs, i finally figured out how they work :)
### 0.1.5
 - Update the Readme so we have some more information on this package 
 - Fix `FetchView` to also support searching child GameObject for views
 - Renamed Dataservice `Load` to `LoadList` since we always load a list of data
### 0.1.0
- The first public release of the package

  
