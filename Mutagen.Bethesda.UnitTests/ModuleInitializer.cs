using System.Runtime.CompilerServices;

namespace Mutagen.Bethesda.UnitTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
        VerifyDiffPlex.Initialize();
    }
}