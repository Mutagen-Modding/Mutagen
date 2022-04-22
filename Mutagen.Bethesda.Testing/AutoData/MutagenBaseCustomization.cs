using AutoFixture;
using Mutagen.Bethesda.Strings.DI;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData;

public class MutagenBaseCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new ModKeyBuilder());
        var makeModExist = new MakeModExist(
            new MakeFileExist());
        var split = new SplitEnumerableIntoSubtypes();
        fixture.Customizations.Add(new ModKeyParameterBuilder(makeModExist));
        fixture.Customizations.Add(new ModKeyMultipleBuilder());
        fixture.Customizations.Add(new ModKeyMultipleParameterBuilder(
            split,
            makeModExist));
        fixture.Customizations.Add(new ModPathParameterBuilder(makeModExist));
        fixture.Customizations.Add(new ModPathMultipleBuilder());
        fixture.Customizations.Add(new ModListingBuilder());
        fixture.Customizations.Add(new ModPathBuilder());
        fixture.Customizations.Add(new OrderBuilder());
        fixture.Inject<IMutagenEncodingProvider>(MutagenEncodingProvider.Instance);
    }
}