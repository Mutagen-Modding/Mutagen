using FluentAssertions;
using Mutagen.Bethesda.Assets;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Skyrim.Assets;

public class AssetTypeTests
{
#if NET7_0_OR_GREATER
    public IAssetType GetInstance<TAssetType>()
        where TAssetType : class, IAssetType
    {
        return TAssetType.Instance;
    }

    [Fact]
    public void TestAllImplementationsAreNotNull()
    {
        var methodInfo = typeof(AssetTypeTests).GetMethod("GetInstance");

        foreach (var implementation in typeof(IAssetType).GetInheritingFromInterface())
        {
            var method = methodInfo?.MakeGenericMethod(implementation);

            var instance = method?.Invoke(this, null);

            instance.Should().NotBeNull();
        }

    }

    [Fact]
    public void TestAllImplementationsHaveNoBaseClass()
    {
        var objectType = typeof(object);
        
        foreach (var implementation in typeof(IAssetType).GetInheritingFromInterface())
        {
            implementation.BaseType.Should().Be(objectType);
        }
    }
#endif
}
