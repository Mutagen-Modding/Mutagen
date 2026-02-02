# Development Notes

This file contains helpful information for developers and AI assistants working on this codebase.

## About Mutagen

Mutagen is a C# library for analyzing, modifying, and creating Bethesda mods. It provides strongly typed interfaces and classes for game records with compile-time type safety.

- **Documentation**: https://mutagen-modding.github.io/Mutagen/
- **Discord**: https://discord.gg/53KMEsW

## Project Structure

### Solutions

The repository contains multiple solution files for different purposes:

- **Mutagen.Records.sln** - Main solution that imports everything (use this for full development)
- **Mutagen.Core.sln** - Core library only
- **Mutagen.UnitTests.sln** - Unit tests
- **Mutagen.Records.[Game].sln** - Game-specific solutions (Skyrim, Fallout4, Oblivion, Starfield)
- **Mutagen.Records.Linux.sln** - Linux-compatible subset

### Key Projects

- **Mutagen.Bethesda.Core** - Core library with shared functionality
- **Mutagen.Bethesda.Kernel** - Kernel/foundation types
- **Mutagen.Bethesda.[Game]** - Game-specific implementations (Skyrim, Fallout4, Oblivion, etc.)
- **Mutagen.Bethesda.Generation** - Code generation infrastructure
- **Mutagen.Bethesda.SourceGenerators** - Source generators
- **Mutagen.Bethesda.Autofac** - Autofac DI integration
- **Mutagen.Bethesda.Json** - JSON serialization support

## Building

### Prerequisites

- .NET 8.0 or 9.0 SDK
- Windows (for full solution), Linux/macOS supported with Mutagen.Records.Linux.sln

### Build Commands

```bash
# Build the main solution
dotnet build Mutagen.Records.sln

# Build specific projects
dotnet build Mutagen.Bethesda.Core/Mutagen.Bethesda.Core.csproj
```

**Note**: Initial builds and full rebuilds can take several minutes due to the large amount of generated code in this project. The codebase includes extensive code generation for game record types, which results in large generated files that need to be compiled. This is normal - be patient and allow extra time for build operations to complete.

### Cleaning the Repository

If you encounter build errors (e.g., error code 0x00000001), clean the repository:
```bash
dotnet clean Mutagen.Records.sln
```

## Testing

### Running Tests

```bash
# Run all tests
dotnet test Mutagen.UnitTests.sln

# Run specific test project
dotnet test Mutagen.Bethesda.UnitTests/Mutagen.Bethesda.UnitTests.csproj

# Run tests with filter
dotnet test --filter "FullyQualifiedName~Context"
```

### Test Projects

- **Mutagen.Bethesda.UnitTests** - Main unit tests
- **Mutagen.Bethesda.Core.UnitTests** - Core library unit tests

## Common Issues

### Case Sensitivity in Tests

Some tests may fail on Linux/macOS due to case-sensitive filesystems. The codebase uses `StringsUtility.GetFileName()` for consistent file naming across platforms.

## Code Generation

Many of the public-facing APIs are code-generated. Check the following for generation logic:

- `Mutagen.Bethesda.Generation` - Generation infrastructure
- `Mutagen.Bethesda.[Game].Generator` - Game-specific generators
- `Mutagen.Bethesda.SourceGenerators` - Roslyn source generators

### Running Code Generators

After modifying generation code, you need to run the generators to update the generated files. Generators must be run from their build output directories because they use relative paths to locate project files.

**Important**: First build the generation solution to ensure your changes are compiled:

```bash
dotnet build Mutagen.Records.sln
```

#### Running a Single Game Generator (Faster)

For most changes, you can run just one game's generator to test your modifications quickly:

```bash
# Example: Running the Skyrim generator (adjust .NET version as needed)
cd Mutagen.Bethesda.Skyrim.Generator/bin/Debug/net9.0
./Mutagen.Bethesda.Skyrim.Generator.exe
```

Replace `net9.0` with your installed .NET SDK version (net8.0, net9.0, net10.0, etc.).

Other game generators follow the same pattern:
- `Mutagen.Bethesda.Fallout4.Generator/bin/Debug/net9.0/Mutagen.Bethesda.Fallout4.Generator.exe`
- `Mutagen.Bethesda.Oblivion.Generator/bin/Debug/net9.0/Mutagen.Bethesda.Oblivion.Generator.exe`
- `Mutagen.Bethesda.Starfield.Generator/bin/Debug/net9.0/Mutagen.Bethesda.Starfield.Generator.exe`

#### Running All Game Generators

Once you've verified your changes work with a single game generator, run the full generator to update all games:

```bash
# From repository root
cd Mutagen.Bethesda.Generator.All/bin/Debug/net8.0
./Mutagen.Bethesda.Generator.All.exe
```

#### When to Use Each Generator

- **Single game generator**: Use when making changes specific to one game's definitions, or for quick iteration when testing generation code changes
- **All games generator**: Use when making changes to core generation infrastructure (like `MajorRecord` or other shared base types), or before committing your final changes

#### Why Working Directory Matters

The generators use relative paths like `../../../../Mutagen.Bethesda.{Game}/Records` to locate project files. Running from the build output directory ensures these relative paths resolve correctly to the repository structure.

## Project File Management

### Adding New Files to Projects

**Important**: Most projects in this repository use `<EnableDefaultCompileItems>False</EnableDefaultCompileItems>`, which means all source files must be explicitly listed in the `.csproj` file. This is necessary to properly nest generated code files under their corresponding XML definition files.

When adding new `.cs` files to a project:

1. **Find the correct location** in the `.csproj` file (files are typically grouped by directory/feature)
2. **Add a `<Compile Include="...">` element** with the file path
3. **For generated files only**: Add a `<DependentUpon>...</DependentUpon>` element to nest it under the XML file

Example:
```xml
<!-- Regular file (no nesting) -->
<Compile Include="Plugins\Analysis\DI\MultiModFileReader.cs" />

<!-- Generated file (nested under XML) -->
<Compile Include="Records\SkyrimMod_Generated.cs">
  <DependentUpon>SkyrimMod.xml</DependentUpon>
</Compile>
```

If you create a new file and the build can't find it, check that it's been added to the `.csproj` file.

## Development Workflow

### Always Verify Your Changes

**Important**: After making code changes, always verify the solution builds and tests pass:

```bash
# 1. Clean the solution (if needed)
dotnet clean Mutagen.Records.sln

# 2. Build the solution
dotnet build Mutagen.Records.sln

# 3. Run tests to verify nothing broke
dotnet test Mutagen.UnitTests.sln
```

This ensures your changes don't break the build or existing functionality.

### File System Operations
- **NEVER redirect to `nul`** - On Windows, `2>nul` creates unwanted files that Git tracks
- Use proper null redirection: `2>/dev/null` (works on Windows with bash)
- For temporary files, use `.claude/` subfolder or designated temp directories that are gitignored
- Example: `ls directory 2>/dev/null || echo "Not found"` instead of `dir directory 2>nul`
- **NEVER use `sed` for bulk find/replace** - `sed` does not preserve Windows CRLF line endings, creating massive spurious diffs
  - On Windows, `sed -i` converts CRLF to LF, causing every line to show as changed in git
  - Use targeted edits with the Edit tool instead of global sed replacements
  - If you must do bulk replacements, only use tools that preserve line endings (e.g., PowerShell with `-Raw` and explicit encoding)
  - Example (incorrect): `find . -name "*.cs" -exec sed -i 's/OldName/NewName/g' {} \;` - creates CRLF→LF changes on every touched file
  - Example (correct): Use Edit tool on each file individually, or ask user to use IDE refactoring tools

## Releases

### Packaging
- Packages are automatically generated in `/nupkg` directory when building with `GeneratePackageOnBuild=true`
- Package versions managed centrally via `Directory.Packages.props`

### Release Versioning
- Create release tags using semantic versioning format: `<major>.<minor>.<patch>`
- Always include the patch number, even if it's zero (e.g., `0.52.1`, not `0.52`)
- **Do not prefix with `v`** (e.g., use `0.52.1`, not `v0.52.1`)
- This format is required for GitVersion compatibility

### Creating GitHub Release Drafts
1. Find the last release tag: `git tag --sort=-version:refname`
2. Get commits since last release: `git log --oneline <last-tag>..HEAD`
3. Construct release notes by categorizing commits:
   - **Enhancements**: New features, performance improvements, major changes
   - **Bug Fixes**: Bug fixes and corrections
   - **Testing & Documentation**: Test additions, documentation updates
4. Create draft release: `gh release create <version> --draft --title "<version>" --notes "<release-notes>"`
5. Include full changelog link: `**Full Changelog**: https://github.com/Mutagen-Modding/Mutagen/compare/<last-tag>...<new-tag>`

## Testing Best Practices

### Using AutoFixture for Test Data

Mutagen uses AutoFixture with custom builders to automatically generate properly configured test data. This is the **preferred approach** for writing tests.

**Key Points:**
- Try to use `[Theory, MutagenAutoData]` attribute for tests that need ModKeys or other primitives
- If you want, you can use `[Theory, MutagenModAutoData]` which will allow injection of mods, and records that are added to the latest mod.
- AutoFixture will inject properly configured `SkyrimMod`, `ModKey`, `FormKey`, etc. as test parameters

## Coding Practices

### Avoid Using `dynamic`

Do not use `dynamic` when possible. The codebase provides proper interfaces and generic methods that should be used instead. Using `dynamic` bypasses compile-time type checking and can lead to runtime errors.

### Prefer ModPath Over DirectoryPath + ModKey

When working with paths to mod files, prefer using `ModPath` instead of separate `DirectoryPath` and `ModKey` parameters. `ModPath` is essentially a string path that is expected to point to a mod file, and includes the associated `ModKey`.

```cs
// Preferred - uses ModPath
public void ProcessMod(ModPath modPath)
{
    var modKey = modPath.ModKey;
    var filePath = modPath.Path;
    // ...
}

// Avoid - separate parameters
public void ProcessMod(DirectoryPath folder, ModKey modKey)
{
    var filePath = Path.Combine(folder.Path, modKey.FileName);
    // ...
}
```

`ModPath` provides better API ergonomics and ensures the path and ModKey stay in sync.

## Contributing

See the main README.md and official documentation for contribution guidelines.