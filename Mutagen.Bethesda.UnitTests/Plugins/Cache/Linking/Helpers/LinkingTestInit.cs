using System;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers
{
    public class LinkingTestInit : IDisposable
    {
        public LinkingTestInit()
        {
            WarmupAll.Init();
        }

        public void Dispose()
        {
        }
    }
}