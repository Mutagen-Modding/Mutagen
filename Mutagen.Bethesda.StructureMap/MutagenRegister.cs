using System;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Masters.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records.DI;
using StructureMap;

namespace Mutagen.Bethesda.StructureMap
{
    public class MutagenRegister : Registry
    {
        public MutagenRegister()
        {
            Scan(s =>
            {
                s.AssemblyContainingType<IArchiveReaderProvider>();
                s.IncludeNamespaceContainingType<IArchiveReaderProvider>();
                s.IncludeNamespaceContainingType<IDataDirectoryLookup>();
                s.IncludeNamespaceContainingType<IImplicitBaseMasterProvider>();
                s.IncludeNamespaceContainingType<ILoadOrderWriter>();
                s.IncludeNamespaceContainingType<IModActivator>();
                s.IncludeNamespaceContainingType<IMasterReferenceReaderFactory>();
                
                s.ExcludeType<GameReleaseInjection>();
                s.ExcludeType<DataDirectoryInjection>();
                
                s.Convention<Convention>();
            });
            ForConcreteType<GameLocator>().Configure.Singleton();
            Forward<GameLocator, IGameDirectoryLookup>();
            Forward<GameLocator, IDataDirectoryLookup>();
        }
    }
}