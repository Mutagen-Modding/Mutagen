using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Inis.DI;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using StrongInject;

namespace Mutagen.Bethesda.Plugins.Utility;

[RegisterModule(typeof(IniModule))]
[RegisterModule(typeof(ImplicitModule))]
[RegisterModule(typeof(EnvironmentsModule))]
[RegisterModule(typeof(ArchiveModule))]
[RegisterModule(typeof(OrderModule))]
internal class MutagenStrongInjectModule
{
}