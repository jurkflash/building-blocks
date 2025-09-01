# Dependency Management

- All NuGet package versions are managed in Directory.Packages.props.
- Use `dotnet restore` to fetch dependencies.
- EF Core migrations should be managed per project; see project README for details.
- Avoid direct package versioning in .csproj files.
- For new dependencies, add them to Directory.Packages.props and reference in the project .csproj.
