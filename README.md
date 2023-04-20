# ZeroUnit

`ZeroUnit` is a Roslyn-powered build-time unit-testing library for .NET. Generates a `Program.cs` that runs your tests. No `Microsoft.NET.Test.Sdk` needed. No runtime-dependencies.

## Installing

Make the `.csproj` with your tests a console app and reference the `ZeroUnit` package:

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <!-- ... -->
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="ZeroUnit"
    Version="1.0.0"
    ReferenceOutputAssembly="false"
    OutputItemType="Analyzer" />
</ItemGroup>
```

Delete `Program.cs` if it exists.

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