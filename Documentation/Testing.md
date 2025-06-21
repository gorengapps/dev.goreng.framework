# Testing

This document describes the testing infrastructure and best practices for The Framework.

## Overview

The Framework includes a comprehensive test suite using Unity's Test Framework (UTF) to ensure reliability and stability. Tests are organized into different categories to cover various aspects of the framework.

## Test Structure

Tests are located in the `Tests/Runtime/` directory and are organized by functionality:

- **FrameworkTests.cs** - Core framework functionality tests
- **TaskExtensionsTests.cs** - Extension method tests for async operations
- **AttributeTests.cs** - Attribute functionality tests

## Running Tests

### Prerequisites

- Unity 2022.3.x LTS
- Unity Test Framework package (automatically included)

### Running in Unity Editor

1. Open Unity Test Runner window: `Window > General > Test Runner`
2. Select the `PlayMode` or `EditMode` tab
3. Click `Run All` to execute all tests
4. Or select individual test suites/cases to run specific tests

### Command Line Testing

For CI/CD pipelines, tests can be run via command line:

```bash
Unity -batchmode -runTests -projectPath /path/to/project -testResults /path/to/results.xml -testPlatform playmode
```

## Writing Tests

### Test Organization

Follow these conventions when adding new tests:

```csharp
using NUnit.Framework;
using Frame.Runtime;

namespace Frame.Tests
{
    [TestFixture]
    public class YourFeatureTests
    {
        [SetUp]
        public void SetUp()
        {
            // Initialize test environment
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after tests
        }

        [Test]
        public void YourMethod_WithValidInput_ShouldReturnExpectedResult()
        {
            // Arrange
            var input = "test";
            var expected = "expected";

            // Act
            var result = YourMethod(input);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
```

### Async Testing

For testing async operations with Unity's Awaitable:

```csharp
[UnityTest]
public IEnumerator AsyncMethod_ShouldCompleteSuccessfully()
{
    // Arrange
    bool completed = false;

    // Act
    StartCoroutine(TestAsync());

    IEnumerator TestAsync()
    {
        var awaitable = SomeAsyncMethod();
        yield return awaitable;
        completed = true;
    }

    // Wait for completion
    yield return new WaitUntil(() => completed);

    // Assert
    Assert.IsTrue(completed);
}
```

### Dependency Injection Testing

When testing components that use dependency injection:

```csharp
[Test]
public void Component_WithDependencies_ShouldInjectCorrectly()
{
    // Arrange
    var container = new DependenciesContainer();
    Framework.RegisterBaseDependencies(container);
    var provider = container.Make();

    // Act
    var service = provider.Get<INavigationService>();

    // Assert
    Assert.IsNotNull(service);
    Assert.IsInstanceOf<NavigationService>(service);
}
```

## Test Categories

### Unit Tests
- Test individual components in isolation
- Mock dependencies when necessary
- Fast execution
- Located in `Tests/Runtime/`

### Integration Tests
- Test component interactions
- Use real dependencies when possible
- Test framework initialization and lifecycle
- Also in `Tests/Runtime/` but marked with `[Category("Integration")]`

### Performance Tests
- Measure execution time for critical paths
- Memory allocation testing
- Use Unity's Performance Testing Package when needed

## Best Practices

1. **Naming Convention**: Use descriptive test names following the pattern `MethodName_Scenario_ExpectedResult`

2. **AAA Pattern**: Structure tests with Arrange, Act, Assert sections

3. **Single Responsibility**: Each test should verify one specific behavior

4. **Test Independence**: Tests should not depend on execution order

5. **Clean Up**: Always clean up resources in TearDown methods

6. **Mock External Dependencies**: Use mocks for external systems and Unity-specific components when needed

## Continuous Integration

Tests are automatically run on:
- Pull request creation
- Main branch commits  
- Release builds

Test results are reported and must pass before merging changes.

## Coverage

Aim for high test coverage on:
- Core framework functionality
- Public APIs
- Critical business logic
- Error handling paths

Use Unity's Code Coverage package to measure and improve test coverage.