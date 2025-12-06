# Data Service

The `DataService` provides a wrapper around Unity's Addressables system, offering standardized ways to load single assets, lists of assets, and instantiate GameObjects with automatic memory management.

## Usage

Inject `IDataService` into your classes:

```csharp
[InjectField] private IDataService _dataService;
```

### Loading Assets

```csharp
// Load a single asset
var sprite = await _dataService.LoadAssetAsync<Sprite>("MySprite");

// Load a list of assets
var items = await _dataService.LoadListAsync<ItemData>("ItemsLabel");

// Load list and extract components
var weapons = await _dataService.LoadListAsyncAs<Weapon>("WeaponsLabel");
```

### Instantiating Objects (Auto-Release)

The `LoadAndInstantiateAsync` method provides "Automagic" memory management.

```csharp
// Loads "MyEnemy", Instantiates it, and returns the EnemyController component
var enemy = await _dataService.LoadAndInstantiateAsync<EnemyController>("MyEnemy");
```

**Key Feature**: Objects instantiated this way have an `AddressableReleaser` component attached. When you `Destroy()` the GameObject, the underlying Addressable asset is automatically released.

### Manual Instantiation (Important)

If you use `LoadAssetAsync<GameObject>` and then manually call `Object.Instantiate`:

```csharp
var prefab = await _dataService.LoadAssetAsync<GameObject>("MyPrefab");
var instance = Object.Instantiate(prefab);
```

> [!WARNING]
> You are responsible for the memory lifecycle in this case. Destroying the `instance` **will not** release the `prefab` handle. You must manually manage the release of the `prefab` asset when you are done with it.

## Memory Management

### Service Disposal

The `DataService` implements `IDisposable`. It tracks all assets loaded via `LoadAssetAsync` and `LoadListAsync`.

```csharp
// Releases all tracked assets
(_dataService as IDisposable)?.Dispose();
```

> [!NOTE]
> Objects created via `LoadAndInstantiateAsync` are **excluded** from this disposal logic. Their ownership is transferred to the instantiated GameObject, so they persist until that specific GameObject is destroyed.
