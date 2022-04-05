using System;
using Mutagen.Bethesda.Plugins.Records.Internals;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;

public class LinkingTestInit : IDisposable
{
    public IMetaInterfaceMapGetter LinkInterfaceMapping = MetaInterfaceMapping.Instance;
        
    public LinkingTestInit()
    {
    }

    public void Dispose()
    {
    }
}