using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Inis.DI;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records.DI;
using Mutagen.Bethesda.Strings.DI;
using StrongInject;

namespace Mutagen.Bethesda.Plugins.Utility;

[RegisterModule(typeof(IniModule))]
[RegisterModule(typeof(ImplicitModule))]
[RegisterModule(typeof(EnvironmentsModule))]
[RegisterModule(typeof(ArchiveModule))]
[RegisterModule(typeof(OrderModule))]
[RegisterModule(typeof(InstallsModule))]
[RegisterModule(typeof(LoadOrderModule))]
[RegisterModule(typeof(RecordsModule))]
[RegisterModule(typeof(StringsModule))]
internal class MutagenStrongInjectModule
{
}