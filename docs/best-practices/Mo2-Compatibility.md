# Mod Organizer 2 Compatibility

This might be fixed at some point, but current versions of Mo2 don't play well with a [new feature](https://learn.microsoft.com/en-us/dotnet/core/compatibility/interop/9.0/cet-support) of DotNet 9 which is enabled by default.

Fix so far has been to disable it by adding `<CETCompat>false</CETCompat>` to the `.csproj` properties.