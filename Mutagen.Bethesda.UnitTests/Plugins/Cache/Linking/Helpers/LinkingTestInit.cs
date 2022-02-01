using System;
using Mutagen.Bethesda.Plugins.Records.Internals;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers
{
    public class LinkingTestInit : IDisposable
    {
        public ILinkInterfaceMapGetter LinkInterfaceMapping = LinkInterfaceMapper.AutomaticFactory();
        
        public LinkingTestInit()
        {
        }

        public void Dispose()
        {
        }
    }
}