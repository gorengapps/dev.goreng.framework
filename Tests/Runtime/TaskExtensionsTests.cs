using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Frame.Runtime.Extensions;
using System.Threading.Tasks;

namespace Frame.Tests
{
    /// <summary>
    /// Tests for TaskExtensions functionality
    /// </summary>
    [TestFixture]
    public class TaskExtensionsTests
    {
        [UnityTest]
        public IEnumerator AsAwaitable_WithCompletedTask_ShouldComplete()
        {
            // Arrange
            var completedTask = Task.CompletedTask;
            bool isCompleted = false;

            // Act
            var awaitable = completedTask.AsAwaitable();
            
            // Start async operation
            StartCoroutine(WaitForAwaitable());

            IEnumerator WaitForAwaitable()
            {
                yield return awaitable;
                isCompleted = true;
            }

            // Wait a frame to ensure coroutine runs
            yield return null;

            // Assert
            Assert.IsTrue(isCompleted, "Awaitable should complete for completed task");
        }

        [UnityTest]
        public IEnumerator AsAwaitableGeneric_WithCompletedTask_ShouldReturnValue()
        {
            // Arrange
            const int expectedValue = 42;
            var completedTask = Task.FromResult(expectedValue);
            int actualValue = 0;

            // Act
            var awaitable = completedTask.AsAwaitable();
            
            // Start async operation
            StartCoroutine(WaitForAwaitable());

            IEnumerator WaitForAwaitable()
            {
                actualValue = yield return awaitable;
            }

            // Wait a frame to ensure coroutine runs
            yield return null;

            // Assert
            Assert.AreEqual(expectedValue, actualValue, "Awaitable should return correct value");
        }

        [UnityTest]
        public IEnumerator AsTask_WithEndOfFrameAwaitable_ShouldComplete()
        {
            // Arrange
            var awaitable = Awaitable.EndOfFrameAsync();
            bool isCompleted = false;

            // Act
            StartCoroutine(WaitForTask());

            IEnumerator WaitForTask()
            {
                var task = awaitable.AsTask();
                yield return new WaitUntil(() => task.IsCompleted);
                isCompleted = task.IsCompletedSuccessfully;
            }

            // Wait a few frames to ensure operation completes
            yield return null;
            yield return null;

            // Assert
            Assert.IsTrue(isCompleted, "Task should complete successfully from Awaitable");
        }

        [Test]
        public void AsTask_WithCompletedAwaitable_ShouldNotThrow()
        {
            // Arrange
            var awaitable = Awaitable.FromResult(42);

            // Act & Assert
            Assert.DoesNotThrow(() => awaitable.AsTask(), 
                "AsTask should not throw with completed awaitable");
        }
    }
}