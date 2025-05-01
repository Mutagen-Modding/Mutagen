using StrongInject;

namespace Mutagen.Bethesda.Plugins.Records.DI;

[Register(typeof(ModImporter<>), typeof(IModImporter<>))]
internal class RecordsModule
{
}