using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Shouldly;
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class ToLinkTests
{
    [Theory, MutagenModAutoData]
    public void ToLink(Npc n)
    {
        n.ToLink().ShouldBeOfType<FormLink<Npc>>();
        n.AsLink().ShouldBeOfType<FormLink<Npc>>();
        IMajorRecordGetter maj = n;
        maj.ToLink().ShouldBeOfType<FormLink<IMajorRecordGetter>>();
        maj.AsLink().ShouldBeOfType<FormLink<IMajorRecordGetter>>();
    }
    
    [Theory, MutagenModAutoData]
    public void ToLinkGetter(Npc n)
    {
        n.ToLinkGetter().ShouldBeOfType<FormLinkGetter<Npc>>();
        n.AsLinkGetter().ShouldBeOfType<FormLinkGetter<Npc>>();
        IMajorRecordGetter maj = n;
        maj.ToLinkGetter().ShouldBeOfType<FormLinkGetter<IMajorRecordGetter>>();
        maj.AsLinkGetter().ShouldBeOfType<FormLinkGetter<IMajorRecordGetter>>();
    }
    
    [Theory, MutagenModAutoData]
    public void ToNullableLink(Npc n)
    {
        n.ToNullableLink().ShouldBeOfType<FormLinkNullable<Npc>>();
        n.AsNullableLink().ShouldBeOfType<FormLinkNullable<Npc>>();
        IMajorRecordGetter maj = n;
        maj.ToNullableLink().ShouldBeOfType<FormLinkNullable<IMajorRecordGetter>>();
        maj.AsNullableLink().ShouldBeOfType<FormLinkNullable<IMajorRecordGetter>>();
    }
    
    [Theory, MutagenModAutoData]
    public void ToNullableLinkGetter(Npc n)
    {
        n.ToNullableLinkGetter().ShouldBeOfType<FormLinkNullableGetter<Npc>>();
        n.AsNullableLinkGetter().ShouldBeOfType<FormLinkNullableGetter<Npc>>();
        IMajorRecordGetter maj = n;
        maj.ToNullableLinkGetter().ShouldBeOfType<FormLinkNullableGetter<IMajorRecordGetter>>();
        maj.AsNullableLinkGetter().ShouldBeOfType<FormLinkNullableGetter<IMajorRecordGetter>>();
    }
}