# Quick Reference Guide

This document provides quick reference examples for common Framework usage patterns.

## Basic Setup

### 1. Initialize the Framework

```csharp
using Frame.Runtime;
using Framework.DI.Container;
using Framework.DI.Provider;

public class GameCore : MonoBehaviour
{
    [SerializeField] private IRef<IDependenciesContainer> _container;
    public static IDependencyProvider provider;
    
    void Start()
    {
        // Register base dependencies
        Framework.RegisterBaseDependencies(_container.value);
        
        // Create provider
        provider = _container.value.Make();
        
        // Initialize framework
        Framework.Initialize(provider);
    }
}
```

### 2. Create a Scene Bootstrap

```csharp
using Frame.Runtime.Bootstrap;
using Frame.Runtime.Navigation;

public partial class MainMenuBootstrap : AbstractBootstrap
{
    [FetchView] private IMainMenuView _mainMenuView;
    
    public override async Awaitable OnBootstrapStartAsync()
    {
        await base.OnBootstrapStartAsync();
        
        // Initialize your scene logic here
        _mainMenuView?.SetTitle("Welcome to My Game");
    }
    
    public override async Awaitable OnBootstrapStopAsync()
    {
        // Cleanup logic here
        await base.OnBootstrapStopAsync();
    }
}
```

### 3. Create a View

```csharp
using Frame.Runtime.Canvas;
using UnityEngine;
using TMPro;

// Interface definition
public interface IMainMenuView : IView
{
    void SetTitle(string title);
    void SetButtonEnabled(bool enabled);
}

// Implementation
public partial class MainMenuView : MonoBehaviour, IMainMenuView
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Button _playButton;
    
    public void SetTitle(string title)
    {
        _titleText.text = title;
    }
    
    public void SetButtonEnabled(bool enabled)
    {
        _playButton.interactable = enabled;
    }
    
    public async Awaitable ViewWillLoadAsync()
    {
        // Initialize view when scene loads
        SetButtonEnabled(false);
        await Awaitable.NextFrameAsync();
        SetButtonEnabled(true);
    }
    
    public async Awaitable ViewWillUnloadAsync()
    {
        // Cleanup when scene unloads
        SetButtonEnabled(false);
    }
}
```

## Navigation

### Load a Scene

```csharp
// In your bootstrap or service
await _navigationService.ShowSceneAsync<GameplayBootstrap>();
```

### Load an Overlay Scene

```csharp
var hudBootstrap = await _navigationService.ShowSceneAsync<HudBootstrap>(setActive: false);
```

### Unload a Scene

```csharp
await _navigationService.UnloadAsync(_sceneContext);
```

## Dependency Injection

### Register Custom Dependencies

```csharp
// In your core setup
_container.value.Register<IMyService, MyService>();
_container.value.Register<IDataRepository>((provider) => new DataRepository(provider.Get<IDataService>()));
```

### Inject Dependencies

```csharp
public class MyController : MonoBehaviour
{
    [InjectField] private IMyService _myService;
    [InjectField] private IDataRepository _dataRepository;
    
    void Awake()
    {
        GameCore.provider.Inject(this);
    }
    
    void Start()
    {
        // Dependencies are now available
        _myService.DoSomething();
    }
}
```

## Data Loading

### Load Assets

```csharp
// Load a single asset
var prefab = await _dataService.LoadAssetAsync<GameObject>("player_prefab");

// Load multiple assets
var weapons = await _dataService.LoadListAsync<WeaponData>("weapons");

// Load and instantiate
var playerInstance = await _dataService.LoadAndInstantiateAsync<PlayerController>("player_prefab");
```

## Common Patterns

### Scene State Management

```csharp
public partial class GameplayBootstrap : AbstractBootstrap
{
    private GameState _currentState;
    
    public override async Awaitable OnBootstrapStartAsync()
    {
        await base.OnBootstrapStartAsync();
        await TransitionToState(GameState.Loading);
    }
    
    private async Awaitable TransitionToState(GameState newState)
    {
        _currentState = newState;
        
        switch (newState)
        {
            case GameState.Loading:
                await LoadGameData();
                await TransitionToState(GameState.Playing);
                break;
                
            case GameState.Playing:
                _gameplayView?.EnableInput(true);
                break;
        }
    }
}
```

### View Communication

```csharp
// Through the bootstrap (recommended)
public partial class UIBootstrap : AbstractBootstrap
{
    [FetchView] private IHealthBarView _healthBar;
    [FetchView] private IScoreView _scoreView;
    
    public void UpdatePlayerStats(int health, int score)
    {
        _healthBar?.SetHealth(health);
        _scoreView?.SetScore(score);
    }
}

// Or through events/services
public class GameEventService : IGameEventService
{
    public event Action<int> OnHealthChanged;
    public event Action<int> OnScoreChanged;
    
    public void UpdateHealth(int health) => OnHealthChanged?.Invoke(health);
    public void UpdateScore(int score) => OnScoreChanged?.Invoke(score);
}
```

### Async Operations

```csharp
// Using Unity's Awaitable
public async Awaitable LoadDataAsync()
{
    var loadOperation = _dataService.LoadAssetAsync<GameData>("game_config");
    var gameData = await loadOperation;
    
    // Process data
    ProcessGameData(gameData);
}

// Converting between Task and Awaitable
public async Awaitable DoNetworkCall()
{
    var httpTask = WebRequest.GetAsync("https://api.example.com/data");
    var result = await httpTask.AsAwaitable();
    return result;
}
```

## Best Practices

1. **Keep Views Dumb**: Views should only handle presentation logic
2. **Use Interfaces**: Always define interfaces for your Views and Services
3. **Async All The Way**: Use async/await consistently throughout your code
4. **Dependency Injection**: Prefer DI over Singleton patterns
5. **Scene Lifecycle**: Always call base methods in bootstrap overrides
6. **Error Handling**: Wrap async operations in try-catch blocks when needed

## Troubleshooting

### Common Issues

1. **Dependencies Not Injected**: Make sure to call `provider.Inject(this)` and that the provider is set up correctly
2. **Views Not Found**: Ensure your view implements the correct interface and is in the scene hierarchy
3. **Scene Loading Fails**: Check that scenes are properly configured as Addressables with the "scenes" label
4. **Bootstrap Not Starting**: Verify that your bootstrap inherits from `AbstractBootstrap` and the scene is loaded correctly