# ZeroUnit

`ZeroUnit` is a zero-dependency unit-testing library for .NET. It generates a `Program.cs` that runs your tests. No `Microsoft.NET.Test.Sdk` or runtime dependencies needed.

## Installing

Make the `.csproj` with your tests a console app and reference the `ZeroUnit` package:

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <!-- ... -->
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="ZeroUnit"
    Version="1.4.0"
    ReferenceOutputAssembly="false"
    OutputItemType="Analyzer"
    ExcludeAssets="all" />
</ItemGroup>
```

Delete the class (usually named `Program`) with the `Main` method if it exists.

## Writing tests

For `ZeroUnit`, a test is any method that:

* is public and in a public class
* has no parameters
* has no return value (or returns a `Task`)
* can be easily called (is either static or in a non-abstract class with a parameterless constructor)

A test passes if it does not throw any exceptions.

Use a parameterless constructor for setup and `IDisposable.Dispose` for teardown if needed.

## Running tests

Run tests with `dotnet run`.

`ZeroUnit` generates a program that runs the tests in parallel. Non-static classes are instantiated for each test. Disposable classes are disposed.

## Global setup and teardown

Add a `Program` class like this:

```csharp
partial class Program
{
    static async Task<int> Main()
    {
        // global setup
        var failedTestCount = await RunTestsAsync();
        // global teardown
        return failedTestCount;
    }

    // ZeroUnit will implement this
    private static partial Task<int> RunTestsAsync();
}
```
