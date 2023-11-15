 # The Framework
 
The framework consists of various elements to make your Unity life a bit easier. 
If you try to fight the framework you will have a hard time. 
But extending it is quite easy.
## Documentation

- [Dependency Injection](/Documentation/DependencyInjection.md)
- Bootstrap (T.B.A)
- Canvas (T.B.A)
## Requirments

- Unity 2022.3.x LTS
## Installation

Add a new "Scoped Registery" to your package manager and use the following url
https://packages.goreng.dev you should be able to see The Framework package in your list.
you can use the scope `dev.goreng`
## Changelog

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

  
