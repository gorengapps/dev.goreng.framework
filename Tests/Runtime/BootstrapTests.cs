using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Frame.Runtime.Bootstrap;
using Frame.Runtime.Canvas;
using Frame.Runtime;
using Framework.DI.Container;
using Framework.DI.Provider;

namespace Frame.Tests
{
    /// <summary>
    /// Tests for Bootstrap functionality
    /// </summary>
    [TestFixture]
    public class BootstrapTests
    {
        private GameObject _testGameObject;
        private TestBootstrap _testBootstrap;
        private IDependencyProvider _provider;

        [SetUp]
        public void SetUp()
        {
            // Create test environment
            _testGameObject = new GameObject("TestBootstrap");
            _testBootstrap = _testGameObject.AddComponent<TestBootstrap>();
            
            // Setup dependency provider
            var container = new DependenciesContainer();
            Frame.Runtime.Framework.RegisterBaseDependencies(container);
            _provider = container.Make();
            
            // Set the provider for the bootstrap system
            IBootstrap.SetProvider(_provider);
        }

        [TearDown]
        public void TearDown()
        {
            if (_testGameObject != null)
            {
                Object.DestroyImmediate(_testGameObject);
            }
            _testGameObject = null;
            _testBootstrap = null;
            _provider = null;
        }

        [Test]
        public void Bootstrap_ShouldImplementInterface()
        {
            // Assert
            Assert.IsNotNull(_testBootstrap, "Bootstrap should be instantiable");
            Assert.IsInstanceOf<IBootstrap>(_testBootstrap, "Bootstrap should implement IBootstrap");
        }

        [UnityTest]
        public IEnumerator OnBootstrapStartAsync_ShouldComplete()
        {
            // Arrange
            bool completed = false;

            // Act
            yield return StartCoroutine(TestBootstrapStart());

            IEnumerator TestBootstrapStart()
            {
                var awaitable = _testBootstrap.OnBootstrapStartAsync();
                yield return awaitable;
                completed = true;
            }

            // Assert
            Assert.IsTrue(completed, "OnBootstrapStartAsync should complete");
        }

        [UnityTest]
        public IEnumerator OnBootstrapStopAsync_ShouldComplete()
        {
            // Arrange
            bool completed = false;

            // Act
            yield return StartCoroutine(TestBootstrapStop());

            IEnumerator TestBootstrapStop()
            {
                var awaitable = _testBootstrap.OnBootstrapStopAsync();
                yield return awaitable;
                completed = true;
            }

            // Assert
            Assert.IsTrue(completed, "OnBootstrapStopAsync should complete");
        }

        [Test]
        public void FetchView_WithNonExistentView_ShouldReturnNull()
        {
            // Act
            var result = _testBootstrap.FetchView<ITestView>();

            // Assert
            Assert.IsNull(result, "FetchView should return null for non-existent view");
        }

        [Test]
        public void UnloadAsync_ShouldReturnAwaitable()
        {
            // Act
            var result = _testBootstrap.UnloadAsync();

            // Assert
            Assert.IsNotNull(result, "UnloadAsync should return an Awaitable");
        }

        [Test]
        public void SceneWillLoadAsync_ShouldReturnAwaitable()
        {
            // Act
            var result = _testBootstrap.SceneWillLoadAsync();

            // Assert
            Assert.IsNotNull(result, "SceneWillLoadAsync should return an Awaitable");
        }

        [Test]
        public void SceneWillUnloadAsync_ShouldReturnAwaitable()
        {
            // Act
            var result = _testBootstrap.SceneWillUnloadAsync();

            // Assert
            Assert.IsNotNull(result, "SceneWillUnloadAsync should return an Awaitable");
        }
    }

    /// <summary>
    /// Test implementation of AbstractBootstrap for testing
    /// </summary>
    public class TestBootstrap : AbstractBootstrap
    {
        // This class inherits all the functionality from AbstractBootstrap
        // and can be used for testing the base implementation
        
        public override async Awaitable OnBootstrapStartAsync()
        {
            await base.OnBootstrapStartAsync();
        }

        public override async Awaitable OnBootstrapStopAsync()
        {
            await base.OnBootstrapStopAsync();
        }
    }

    /// <summary>
    /// Test interface for view testing
    /// </summary>
    public interface ITestView : IView
    {
        void TestMethod();
    }

    /// <summary>
    /// Test implementation of IView for testing
    /// </summary>
    public class TestView : MonoBehaviour, ITestView
    {
        public void TestMethod()
        {
            // Test implementation
        }

        public Awaitable ViewWillLoadAsync()
        {
            return Awaitable.FromResult(true);
        }

        public Awaitable ViewWillUnloadAsync()
        {
            return Awaitable.FromResult(true);
        }
    }
}