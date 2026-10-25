# Development Notes

This file contains helpful information for developers working on this codebase.

## About Mutagen

Mutagen is a C# library for analyzing, modifying, and creating Bethesda mods. It provides strongly typed interfaces and classes for game records with compile-time type safety.

- **Documentation**: https://mutagen-modding.github.io/Mutagen/
- **Discord**: https://discord.gg/53KMEsW

## Solutions

The repository has several solutions that load different subsets of the projects. The full solution is large and slow to build, so a smaller one is often more convenient.

| Solution | Contains |
| --- | --- |
| `Mutagen.Records.sln` | Everything |
| `Mutagen.Records.Linux.sln` | Everything except the WPF projects, which only build on Windows |
| `Mutagen.Records.[Game].sln` | The core plus a single game (Skyrim, Fallout3, Fallout4, Oblivion, Starfield) |
| `Mutagen.Core.sln` | The core library only |
| `Mutagen.UnitTests.sln` | The unit test projects |

## Developer Guides

These live alongside the rest of the documentation, under Contributing:

- [Code Generation](docs/Contributing/development/Code-Generation.md)
- [Testing](docs/Contributing/development/Testing.md)
- [Code Conventions](docs/Contributing/development/Code-Conventions.md)

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

## Contributing

See the main README.md and official documentation for contribution guidelines.