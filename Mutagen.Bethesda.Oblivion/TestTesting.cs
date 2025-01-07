using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Oblivion;

public class TestTesting
{
    public void Test(Func<Stream> streams)
    {
        OblivionMod.Create
            .FromPath("SomePath");
        var mod = OblivionMod.Create
            .FromStreamFactory(streams, ModKey.Null)
            .SingleThread()
            .Mutable()
            .Construct();
    }
}