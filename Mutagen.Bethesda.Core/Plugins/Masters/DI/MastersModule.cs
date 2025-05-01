using StrongInject;

namespace Mutagen.Bethesda.Plugins.Masters.DI;

[Register<TransitiveMasterLocator, ITransitiveMasterLocator>]
[Register<MasterReferenceReaderFactory, IMasterReferenceReaderFactory>]
[Register<MasterFlagsLookupCompiler, IMasterFlagsLookupCompiler>]
[Register<KeyedMasterStyleReader, IKeyedMasterStyleReader>]
internal class MastersModule
{
    
}