using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Mutagen.Bethesda.Core.UnitTests;
using Mutagen.Bethesda.SourceGenerators.CustomAspectInterface;
using Xunit;
using VerifyCS = Mutagen.Bethesda.UnitTests.SourceGenerators.CSharpSourceGeneratorVerifier<Mutagen.Bethesda.SourceGenerators.CustomAspectInterface.CustomAspectInterfaceGenerator>;

namespace Mutagen.Bethesda.UnitTests.SourceGenerators
{
    public class CustomAspectGeneratorTests: IClassFixture<LoadMetadataReferenceFixture>
    {
        private readonly LoadMetadataReferenceFixture metadata;

        public CustomAspectGeneratorTests(LoadMetadataReferenceFixture metadata)
        {
            this.metadata = metadata;
        }
        
        [Fact]
        public async Task TestSourceGeneratorWithEnvironmentVariables()
        {
            const string source = @"
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Core.Plugins.Aspects;

namespace SomeNamespace
{
    public partial class Class
    {
        static void Test()
        {
            IArmor armor = new Armor(FormKey.Null, SkyrimRelease.SkyrimSE);
            MyCustomFunction(armor);
        }

        public static void MyCustomFunction(ISomeCustomAspect customAspect)
        {
            customAspect.PickUpSound.Clear();
        }
    }

    [CustomAspectInterface(typeof(IArmor))]
    public interface ISomeCustomAspect
    {
        public IFormLinkNullable<ISoundDescriptorGetter> PickUpSound { get; }
    }
}
";
            
            const string generated = @"using Mutagen.Bethesda.Skyrim;

namespace SomeNamespace
{
    #region Wrappers
    public class IArmorWrapper : ISomeCustomAspect
    {
        private readonly IArmor _wrapped;
        public Mutagen.Bethesda.Plugins.IFormLinkNullable<Mutagen.Bethesda.Skyrim.ISoundDescriptorGetter> PickUpSound
        {
            get => _wrapped.PickUpSound;
        }

        public IArmorWrapper(IArmor rhs)
        {
            _wrapped = rhs;
        }
    }


    #endregion

    #region Mix Ins
    public static class WrapperMixIns
    {
        public static IArmorWrapper AsISomeCustomAspect(this IArmor rhs)
        {
            return new IArmorWrapper(rhs);
        }

    }
    #endregion

}
";
            var testState = new VerifyCS.Test
            {
                TestState =
                {
                    Sources =
                    {
                        ("SomeFile.cs", source)
                    },
                    GeneratedSources =
                    {
                        (typeof(CustomAspectInterfaceGenerator), "CustomAspectInterfaces.g.cs", generated)
                    },
                },
                CompilerDiagnostics = CompilerDiagnostics.None,
            };
            foreach (var meta in metadata.MetadataReferences)
            {
                testState.TestState.AdditionalReferences.Add(meta);
            }
            await testState.RunAsync();
        }
    }
}