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

## Contributing

See the main README.md and official documentation for contribution guidelines.