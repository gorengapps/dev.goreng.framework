using NUnit.Framework;
using Frame.Runtime;
using Frame.Runtime.Data;
using Frame.Runtime.Navigation;
using Framework.DI.Container;
using Framework.Loop;

namespace Frame.Tests
{
    /// <summary>
    /// Tests for the core Framework functionality
    /// </summary>
    [TestFixture]
    public class FrameworkTests
    {
        private IDependenciesContainer _container;

        [SetUp]
        public void SetUp()
        {
            // Create a fresh container for each test
            _container = new DependenciesContainer();
        }

        [TearDown]
        public void TearDown()
        {
            _container = null;
        }

        [Test]
        public void RegisterBaseDependencies_ShouldRegisterAllRequiredServices()
        {
            // Act
            Framework.RegisterBaseDependencies(_container);
            var provider = _container.Make();

            // Assert
            Assert.IsNotNull(provider.Get<IDataService>(), "IDataService should be registered");
            Assert.IsNotNull(provider.Get<INavigationService>(), "INavigationService should be registered");
            Assert.IsNotNull(provider.Get<IRunLoop>(), "IRunLoop should be registered");
        }

        [Test]
        public void RegisterBaseDependencies_ShouldRegisterCorrectImplementations()
        {
            // Act
            Framework.RegisterBaseDependencies(_container);
            var provider = _container.Make();

            // Assert
            Assert.IsInstanceOf<DataService>(provider.Get<IDataService>(), 
                "IDataService should resolve to DataService implementation");
            Assert.IsInstanceOf<NavigationService>(provider.Get<INavigationService>(), 
                "INavigationService should resolve to NavigationService implementation");
        }

        [Test]
        public void Initialize_ShouldNotThrow()
        {
            // Arrange
            Framework.RegisterBaseDependencies(_container);
            var provider = _container.Make();

            // Act & Assert
            Assert.DoesNotThrow(() => Framework.Initialize(provider), 
                "Framework.Initialize should not throw with valid provider");
        }

        [Test]
        public void Framework_ShouldHandleNullProvider()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => Framework.Initialize(null), 
                "Framework.Initialize should handle null provider gracefully");
        }
    }
}