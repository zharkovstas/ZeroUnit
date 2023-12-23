param (
    [string]$Version
)

if (-not $Version) {
    throw "-Version is required."
}

dotnet clean `
&& dotnet build -p:Configuration=Release `
&& dotnet run --no-build -c Release --project .\src\ZeroUnit.Tests `
&& dotnet run --no-build -c Release --project .\src\ZeroUnit.Example.Tests `
&& dotnet pack --no-build -p:Configuration=Release -p:PackageVersion=$Version .\src\ZeroUnit\
