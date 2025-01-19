using StrongInject;

namespace Mutagen.Bethesda.Environments.DI;

[Register(typeof(GameDirectoryProvider), typeof(IGameDirectoryProvider))]
[Register(typeof(GameCategoryContext), typeof(IGameCategoryContext))]
internal class EnvironmentsModule
{
}