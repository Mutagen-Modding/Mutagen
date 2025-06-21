using Mutagen.Bethesda.Environments.DI;
using Noggog;
using Noggog.Testing.IO;

namespace Mutagen.Bethesda.Testing.Fakes;

public class ManualDataDirectoryProvider : IDataDirectoryProvider
{
    public DirectoryPath Path { get; set; } = $"{PathingUtil.DrivePrefix}DataDirectory";
}