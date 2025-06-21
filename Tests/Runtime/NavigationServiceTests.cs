using NUnit.Framework;
using UnityEngine;
using Frame.Runtime.Navigation;
using Frame.Runtime.Bootstrap;
using Framework.DI.Container;
using System.Collections.Generic;
using System;

namespace Frame.Tests
{
    /// <summary>
    /// Tests for NavigationService functionality
    /// </summary>
    [TestFixture]
    public class NavigationServiceTests
    {
        private INavigationService _navigationService;

        [SetUp]
        public void SetUp()
        {
            _navigationService = new NavigationService();
        }

        [TearDown]
        public void TearDown()
        {
            _navigationService = null;
        }

        [Test]
        public void NavigationService_ShouldImplementInterface()
        {
            // Assert
            Assert.IsNotNull(_navigationService, "NavigationService should be instantiable");
            Assert.IsInstanceOf<INavigationService>(_navigationService, "NavigationService should implement INavigationService");
        }

        [Test]
        public void NavigationService_ShouldBeRegistrable()
        {
            // Arrange
            var container = new DependenciesContainer();
            
            // Act
            container.Register<INavigationService, NavigationService>();
            var provider = container.Make();
            var service = provider.Get<INavigationService>();

            // Assert
            Assert.IsNotNull(service, "NavigationService should be registrable and resolvable");
            Assert.IsInstanceOf<NavigationService>(service, "Resolved service should be NavigationService instance");
        }

        [Test]
        public void Initialise_ShouldAcceptSceneMapping()
        {
            // Arrange
            var sceneMapping = new Dictionary<string, Type>
            {
                { "test_scene", typeof(MockBootstrap) }
            };

            // Act & Assert
            Assert.DoesNotThrow(() => _navigationService.Initialise(sceneMapping), 
                "Initialise should accept valid scene mapping without throwing");
        }

        [Test]
        public void Initialise_ShouldHandleEmptyMapping()
        {
            // Arrange
            var emptyMapping = new Dictionary<string, Type>();

            // Act & Assert
            Assert.DoesNotThrow(() => _navigationService.Initialise(emptyMapping), 
                "Initialise should handle empty scene mapping");
        }

        [Test]
        public void Initialise_ShouldHandleNullMapping()
        {
            // Act & Assert - This might throw depending on implementation, but shouldn't crash
            try
            {
                _navigationService.Initialise(null);
                Assert.IsTrue(true, "NavigationService handled null mapping gracefully");
            }
            catch (ArgumentNullException)
            {
                Assert.IsTrue(true, "NavigationService correctly throws ArgumentNullException for null mapping");
            }
            catch (System.Exception ex)
            {
                Assert.Fail($"NavigationService should either handle null mapping or throw ArgumentNullException, but threw: {ex.GetType().Name}");
            }
        }

        [Test]
        public void ShowSceneAsync_ShouldReturnAwaitable()
        {
            // Arrange
            var sceneMapping = new Dictionary<string, Type>
            {
                { "MockBootstrap", typeof(MockBootstrap) }
            };
            _navigationService.Initialise(sceneMapping);

            // Act
            var result = _navigationService.ShowSceneAsync<MockBootstrap>();

            // Assert
            Assert.IsNotNull(result, "ShowSceneAsync should return an Awaitable");
        }

        [Test]
        public void GetSceneHandle_WithNonExistentScene_ShouldReturnNull()
        {
            // Arrange
            var sceneMapping = new Dictionary<string, Type>();
            _navigationService.Initialise(sceneMapping);

            // Act
            var result = _navigationService.GetSceneHandle<MockBootstrap>();

            // Assert
            Assert.IsNull(result, "GetSceneHandle should return null for non-existent scene");
        }
    }

    /// <summary>
    /// Mock bootstrap for testing
    /// </summary>
    public class MockBootstrap : MonoBehaviour, IBootstrap
    {
        public Awaitable OnBootstrapStartAsync()
        {
            return Awaitable.FromResult(true);
        }

        public Awaitable OnBootstrapStopAsync()
        {
            return Awaitable.FromResult(true);
        }

        public Awaitable UnloadAsync()
        {
            return Awaitable.FromResult(true);
        }

        public T FetchView<T>() where T : Frame.Runtime.Canvas.IView
        {
            return default(T);
        }

        public Awaitable SceneWillUnloadAsync()
        {
            return Awaitable.FromResult(true);
        }

        public Awaitable SceneWillLoadAsync()
        {
            return Awaitable.FromResult(true);
        }

        public void Load(Frame.Runtime.Scene.IAsyncScene sceneContext)
        {
            // Mock implementation
        }
    }
}