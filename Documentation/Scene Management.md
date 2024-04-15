## Intro

The idea of this framework is to provide an easy to use setup to get a nice scene structure going in Unity. It relies heavily on the Addressable package made by Unity

The package is broken down in a few structures which will be explained further down in this document.

- [Canvas](#Canvas)
- [Bootstrap](#Bootstrap)
## Canvas

A canvas can be seen as a simple `View` that should be as dumb as possible, make sure that you define an interface to make it play nicely with the setup. It is in essence a regular `MonoBehaviour`.

```csharp
// Make sure this implements ICanvas
public interface ISampleCanvas: ICanvas
{
	public void SetLabel(string value);
}

public partial class SampleCanvas: Monobehaviour 
{
	[SerializeField] private TextMeshProUGUI _label;
}

public partial class SampleCanvas: ISampleCanvas 
{
	public void SetLabel(string value) 
	{
		_label.text = value;
	}
	
	public async Task SceneWillLoad() 
	{
		// If your canvas needs support for InjectFields 
		provider.Inject(this);
		
		return Task.completedTask;
	}
	
	public async Task SceneWillLoad() 
	{
		return Task.completedTask;
	}
}
```

## Bootstrap

Every physical Unity scene needs a Bootstrap counterpart in this setup, the bootstrap is where you kick off your scene logic. Under the hood its still a `MonoBehaviour` but there are some caveats that you need to take into consideration.

```csharp
public parial class SampleBootstap: AbstractBootrap 
{
	// Not needed in real app every bootstrap has this service already defined
	[InjectField] INavigationService _navigationService;

	// Will load the canvas automatically from your hierarchy
	[FetchCanvas] ISampleCanvas _sampleCanvas;

	// Called when the bootstrap will load
	public override OnBootstrapStart() 
	{
		// Important to call the base to let its do its magic
		base.OnBootstrapStart();
		
		await ShowHud();
		
		_sampleCanvas?.SetValue("Done Loading");
	}

	// Called when the bootstrap will unload
	public override OnBootstrapStop() 
	{
	}
	
	private async Task ShowHud()
	{
		
		var hudBootstrap = await _navigationService
			.ShowSupplementaryScene<IHudBootstrap>("key", false);
		
		// Do Something with the HUD
	}
}

public partial class SampleBootstrap: ISampleBootstrap 
{

}
```

In the above sample you see a basic structured bootstrap. That once the bootstrap has finished loading it will automatically load the HUD as an overlay scene. 

Since there needs to be some time for the Bootstrap to prepare itself. It will eat up the basic `Start` that is declared in Unity. You can still override / use these functionalities yourself but be aware that all your dependencies will not be resolved yet.

You can view `AbstractBootstrap` to see the implementation in more details.

Its up to the developer to not clutter your `Controller` / `Bootstrap` you can use other patterns to break up logic into chunks (`Repositories` / `DataSources` / `Services` )

## Editor Setup

When declaring a new scene you can just create a regular scene. Once you have a scene in Unity you can right click to create a Framework scene

![[Create Scene.png]]

Once you have created the scene you are ready to configure it. fill in a type (key) for your scene that you reference from code and assign a physical Unity scene to the scene property. Mark the asset as addressable and give it the key `scenes` if it doesn't exist you can define it yourself.

> From version 0.2.5 the scene label will automatically be added when creating a new scene, it will also update the address to if you rename a scene. The type will also be updated to reflect the file name. This makes configuration easier and less error prone

The `NavigationService` will try to load all assets by using the key `scenes`.

![[Configure Scene.png]]