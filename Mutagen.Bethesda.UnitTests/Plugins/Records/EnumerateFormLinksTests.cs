using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class EnumerateFormLinksTests
{
    [Fact]
    public void KeyValueDictionaryRetrieval()
    {
        var key = FormKey.Factory("123456:Skyrim.esm");
        var package = new Package(FormKey.Null, SkyrimRelease.SkyrimSE);
        package.Data[1] = new PackageDataLocation()
        {
            Location = new LocationTargetRadius()
            {
                Target = new LocationTarget()
                {
                    Link = new FormLink<IPlacedGetter>(key)
                }
            }
        };
        package.EnumerateFormLinks().Select(x => x.FormKey).Should().Contain(key);
    }
}