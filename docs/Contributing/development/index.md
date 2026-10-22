# Developing Mutagen

- [Code Generation](Code-Generation.md) covers how the record classes are generated, and how to change them
- [Testing](Testing.md) covers how tests are written
- [Code Conventions](Code-Conventions.md) covers conventions to follow when writing code

## Solutions
The repository has several solutions that load different subsets of the projects.  The full solution is large and slow to build, so a smaller one is often more convenient.

| Solution | Contains |
| --- | --- |
| `Mutagen.Records.sln` | Everything |
| `Mutagen.Records.Linux.sln` | Everything except the WPF projects, which only build on Windows |
| `Mutagen.Records.[Game].sln` | The core plus a single game (Skyrim, Fallout3, Fallout4, Oblivion, Starfield) |
| `Mutagen.Core.sln` | The core library only |
| `Mutagen.UnitTests.sln` | The unit test projects |
