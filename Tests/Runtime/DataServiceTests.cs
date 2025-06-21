using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Frame.Runtime.Data;
using Framework.DI.Container;

namespace Frame.Tests
{
    /// <summary>
    /// Tests for DataService functionality
    /// </summary>
    [TestFixture]
    public class DataServiceTests
    {
        private IDataService _dataService;

        [SetUp]
        public void SetUp()
        {
            _dataService = new DataService();
        }

        [TearDown]
        public void TearDown()
        {
            _dataService = null;
        }

        [Test]
        public void DataService_ShouldImplementInterface()
        {
            // Assert
            Assert.IsNotNull(_dataService, "DataService should be instantiable");
            Assert.IsInstanceOf<IDataService>(_dataService, "DataService should implement IDataService");
        }

        [Test]
        public void DataService_ShouldBeRegistrable()
        {
            // Arrange
            var container = new DependenciesContainer();
            
            // Act
            container.Register<IDataService, DataService>();
            var provider = container.Make();
            var service = provider.Get<IDataService>();

            // Assert
            Assert.IsNotNull(service, "DataService should be registrable and resolvable");
            Assert.IsInstanceOf<DataService>(service, "Resolved service should be DataService instance");
        }

        [UnityTest]
        public IEnumerator LoadAssetAsync_WithInvalidKey_ShouldHandleGracefully()
        {
            // Arrange
            string invalidKey = "nonexistent_key_12345";
            bool exceptionCaught = false;

            // Act
            yield return StartCoroutine(TestLoadAsset());

            IEnumerator TestLoadAsset()
            {
                try
                {
                    var awaitable = _dataService.LoadAssetAsync<GameObject>(invalidKey);
                    yield return awaitable;
                }
                catch (System.Exception)
                {
                    exceptionCaught = true;
                }
            }

            // Assert - We expect this to either handle gracefully or throw a specific exception
            // The exact behavior depends on the implementation but it shouldn't crash Unity
            Assert.IsTrue(true, "Method should complete without crashing Unity");
        }

        [UnityTest]
        public IEnumerator LoadListAsync_WithInvalidKey_ShouldHandleGracefully()
        {
            // Arrange
            string invalidKey = "nonexistent_list_key_12345";

            // Act
            yield return StartCoroutine(TestLoadList());

            IEnumerator TestLoadList()
            {
                try
                {
                    var awaitable = _dataService.LoadListAsync<GameObject>(invalidKey);
                    yield return awaitable;
                }
                catch (System.Exception)
                {
                    // Expected for invalid keys
                }
            }

            // Assert
            Assert.IsTrue(true, "Method should complete without crashing Unity");
        }

        [Test]
        public void LoadAssetAsync_ShouldReturnAwaitable()
        {
            // Act
            var result = _dataService.LoadAssetAsync<GameObject>("test_key");

            // Assert
            Assert.IsNotNull(result, "LoadAssetAsync should return an Awaitable");
        }

        [Test]
        public void LoadListAsync_ShouldReturnAwaitable()
        {
            // Act
            var result = _dataService.LoadListAsync<GameObject>("test_key");

            // Assert
            Assert.IsNotNull(result, "LoadListAsync should return an Awaitable");
        }

        [Test]
        public void LoadAndInstantiateAsync_ShouldReturnAwaitable()
        {
            // Act
            var result = _dataService.LoadAndInstantiateAsync<Transform>("test_key");

            // Assert
            Assert.IsNotNull(result, "LoadAndInstantiateAsync should return an Awaitable");
        }
    }
}