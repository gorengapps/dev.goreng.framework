using NUnit.Framework;
using UnityEngine;
using Frame.Runtime;
using Frame.Runtime.Canvas;

namespace Frame.Tests
{
    /// <summary>
    /// Tests for Framework attributes
    /// </summary>
    [TestFixture]
    public class AttributeTests
    {
        [Test]
        public void FetchViewAttribute_ShouldBeCreatable()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new FetchViewAttribute(), 
                "FetchViewAttribute should be creatable without errors");
        }

        [Test]
        public void FetchViewAttribute_ShouldBeAttribute()
        {
            // Arrange
            var attribute = new FetchViewAttribute();

            // Assert
            Assert.IsInstanceOf<System.Attribute>(attribute, 
                "FetchViewAttribute should inherit from Attribute");
        }

        [Test]
        public void FetchViewAttribute_ShouldHaveCorrectUsage()
        {
            // Arrange
            var attributeType = typeof(FetchViewAttribute);
            
            // Act
            var attributes = attributeType.GetCustomAttributes(typeof(JetBrains.Annotations.UsedImplicitlyAttribute), false);
            
            // Assert
            Assert.IsTrue(attributes.Length > 0, 
                "FetchViewAttribute should have UsedImplicitly attribute");
        }
    }

    /// <summary>
    /// Mock classes for testing attribute functionality
    /// </summary>
    public class MockView : MonoBehaviour, IView
    {
        [FetchView]
        public IView _mockViewField;

        public Awaitable ViewWillLoadAsync()
        {
            return Awaitable.FromResult(true);
        }

        public Awaitable ViewWillUnloadAsync()
        {
            return Awaitable.FromResult(true);
        }
    }

    [TestFixture]
    public class AttributeUsageTests
    {
        [Test]
        public void FetchViewAttribute_ShouldBeApplicableToFields()
        {
            // Arrange
            var mockViewType = typeof(MockView);
            var field = mockViewType.GetField("_mockViewField");

            // Act
            var attributes = field.GetCustomAttributes(typeof(FetchViewAttribute), false);

            // Assert
            Assert.IsTrue(attributes.Length > 0, 
                "FetchViewAttribute should be applicable to fields");
        }
    }
}