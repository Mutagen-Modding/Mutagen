using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Skyrim.Assets;

public class AssetTestsSkyrim
{
    [Fact]
    public void TestRemapBook() {
	    var book = new Book(FormKey.Null, SkyrimRelease.SkyrimSE);

	    book.BookText.String = """
		Some test text
		abc <img src='img://Textures/11.png'>  def
		Textures/11.png
		  <img src='img://Textures/44.png'>
		  <img src='img://Textures/22.png'>
		More random text
		  <img src='img://Textures/33.png'>
		XXX
		""";

	    book.RemapInferredAssetLinks(new Dictionary<IAssetLinkGetter, string>
	    {
		    { new AssetLink<SkyrimTextureAssetType>("Textures/11.png"), "Textures/aa.png" },
		    { new AssetLink<SkyrimTextureAssetType>("Textures/22.png"), "Textures/bb.png" },
		    { new AssetLink<SkyrimTextureAssetType>("Textures/33.png"), "Textures/cc.png" },
		    { new AssetLink<SkyrimTextureAssetType>("Textures/44.png"), "Textures/dd.png" },
	    });

	    book.BookText.String.Should().Be("""
		Some test text
		abc <img src='img://Textures/aa.png'>  def
		Textures/11.png
		  <img src='img://Textures/dd.png'>
		  <img src='img://Textures/bb.png'>
		More random text
		  <img src='img://Textures/cc.png'>
		XXX
		""");
    }
}
