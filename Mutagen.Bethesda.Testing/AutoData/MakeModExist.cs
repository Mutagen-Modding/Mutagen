using System.IO;
using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData
{
    public interface IMakeModExist
    {
        void MakeExist(ModKey modKey, ISpecimenContext context);
    }

    public class MakeModExist : IMakeModExist
    {
        public IMakeFileExist MakeFileExist { get; }

        public MakeModExist(IMakeFileExist makeFileExist)
        {
            MakeFileExist = makeFileExist;
        }
        
        public void MakeExist(ModKey modKey, ISpecimenContext context)
        {
            var dataDir = context.Create<IDataDirectoryProvider>();
            MakeFileExist.MakeExist(Path.Combine(dataDir.Path, modKey.FileName), context);
        }
    }
}