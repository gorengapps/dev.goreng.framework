using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Frame.Runtime;
using Frame.Runtime.Bootstrap;
using Frame.Runtime.Canvas;
using Frame.Runtime.Data;
using Frame.Runtime.Navigation;
using Framework.DI.Container;
using Framework.DI.Provider;

namespace Frame.Tests
{
    /// <summary>
    /// Integration tests for the entire Framework
    /// </summary>
    [TestFixture]
    public class FrameworkIntegrationTests
    {
        private GameObject _coreGameObject;
        private IDependencyProvider _provider;

        [SetUp]
        public void SetUp()
        {
            // Create a test environment that mimics real usage
            _coreGameObject = new GameObject("FrameworkCore");
            
            var container = new DependenciesContainer();
            Frame.Runtime.Framework.RegisterBaseDependencies(container);
            _provider = container.Make();
            
            Frame.Runtime.Framework.Initialize(_provider);
        }

        [TearDown]
        public void TearDown()
        {
            if (_coreGameObject != null)
            {
                Object.DestroyImmediate(_coreGameObject);
            }
            _coreGameObject = null;
            _provider = null;
        }

        [Test]
        public void Framework_FullInitialization_ShouldSucceed()
        {
            // This test verifies that the entire framework can be initialized without errors
            
            // Assert that all core services are available
            Assert.IsNotNull(_provider.Get<IDataService>(), "IDataService should be available");
            Assert.IsNotNull(_provider.Get<INavigationService>(), "INavigationService should be available");
            Assert.IsNotNull(_provider.Get<Framework.Loop.IRunLoop>(), "IRunLoop should be available");
        }

        [UnityTest]
        public IEnumerator Framework_WithRealBootstrap_ShouldWork()
        {
            // Arrange
            var bootstrapGO = new GameObject("TestScene");
            var bootstrap = bootstrapGO.AddComponent<IntegrationTestBootstrap>();
            bool bootstrapStarted = false;

            // Act
            yield return StartCoroutine(TestBootstrapLifecycle());

            IEnumerator TestBootstrapLifecycle()
            {
                var startTask = bootstrap.OnBootstrapStartAsync();
                yield return startTask;
                bootstrapStarted = true;
                
                var stopTask = bootstrap.OnBootstrapStopAsync();
                yield return stopTask;
            }

            // Assert
            Assert.IsTrue(bootstrapStarted, "Bootstrap should start successfully");
            
            // Cleanup
            Object.DestroyImmediate(bootstrapGO);
        }

        [Test]
        public void Framework_DependencyInjection_ShouldWorkEndToEnd()
        {
            // Arrange
            var testGO = new GameObject("TestObject");
            var testComponent = testGO.AddComponent<TestInjectableComponent>();

            // Act
            _provider.Inject(testComponent);

            // Assert
            Assert.IsNotNull(testComponent.GetDataService(), "DataService should be injected");
            Assert.IsNotNull(testComponent.GetNavigationService(), "NavigationService should be injected");
            
            // Cleanup
            Object.DestroyImmediate(testGO);
        }

        [Test]
        public void Framework_ViewSystem_ShouldIntegrateWithBootstrap()
        {
            // Arrange
            var sceneGO = new GameObject("TestScene");
            var viewGO = new GameObject("TestView");
            viewGO.transform.SetParent(sceneGO.transform);
            
            var bootstrap = sceneGO.AddComponent<IntegrationTestBootstrap>();
            var view = viewGO.AddComponent<IntegrationTestView>();

            // Act - This simulates what happens in a real scene
            _provider.Inject(bootstrap);
            _provider.Inject(view);

            // Assert
            Assert.IsNotNull(bootstrap, "Bootstrap should be created");
            Assert.IsNotNull(view, "View should be created");
            Assert.IsInstanceOf<IBootstrap>(bootstrap, "Bootstrap should implement IBootstrap");
            Assert.IsInstanceOf<IView>(view, "View should implement IView");
            
            // Cleanup
            Object.DestroyImmediate(sceneGO);
        }
    }

    /// <summary>
    /// Test Bootstrap for integration testing
    /// </summary>
    public class IntegrationTestBootstrap : AbstractBootstrap
    {
        public bool HasStarted { get; private set; }
        public bool HasStopped { get; private set; }

        public override async Awaitable OnBootstrapStartAsync()
        {
            await base.OnBootstrapStartAsync();
            HasStarted = true;
        }

        public override async Awaitable OnBootstrapStopAsync()
        {
            await base.OnBootstrapStopAsync();
            HasStopped = true;
        }
    }

    /// <summary>
    /// Test View for integration testing
    /// </summary>
    public class IntegrationTestView : MonoBehaviour, IView
    {
        public bool HasLoaded { get; private set; }
        public bool HasUnloaded { get; private set; }

        public async Awaitable ViewWillLoadAsync()
        {
            HasLoaded = true;
        }

        public async Awaitable ViewWillUnloadAsync()
        {
            HasUnloaded = true;
        }
    }

    /// <summary>
    /// Test component that uses dependency injection
    /// </summary>
    public class TestInjectableComponent : MonoBehaviour
    {
        [Framework.DI.InjectField] 
        private IDataService _dataService;
        
        [Framework.DI.InjectField] 
        private INavigationService _navigationService;

        public IDataService GetDataService() => _dataService;
        public INavigationService GetNavigationService() => _navigationService;
    }
}