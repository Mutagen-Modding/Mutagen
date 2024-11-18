using System.Runtime.CompilerServices;
using Autofac.Features.OwnedInstances;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Mutagen.Bethesda.SourceGenerators.CustomAspectInterface;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.SourceGenerators;

public class CustomAspectGeneratorTests: IClassFixture<LoadMetadataReferenceFixture>
{
    public class SourceGenerationTestHelper
    {
        private static bool AutoVerify = false;

        private static VerifySettings GetVerifySettings()
        {
            var verifySettings = new VerifySettings();
    #if DEBUG
            if (AutoVerify)
            {
                verifySettings.AutoVerify();
            }
    #else
            verifySettings.DisableDiff();
    #endif
            return verifySettings;
        }
        
        public static Task VerifySerialization(string source, [CallerFilePath] string sourceFile = "")
        {
            // Parse the provided string into a C# syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
        
            IEnumerable<PortableExecutableReference> references = new[]
            {
                MetadataReference.CreateFromFile(typeof(Owned<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(FilePath).Assembly.Location),
            };
            
            // Create a Roslyn compilation for the syntax tree.
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: references);

            // Create an instance of our incremental source generator
            var generator = new CustomAspectInterfaceGenerator();

            // The GeneratorDriver is used to run our generator against a compilation
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        
            // Run the source generator!
            driver = driver.RunGenerators(compilation);
            
            // Use verify to snapshot test the source generator output!
            return Verifier.Verify(driver, GetVerifySettings(), sourceFile);
        }

        public static GeneratorDriverRunResult RunSourceGenerator(string source)
        {
            // Parse the provided string into a C# syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            IEnumerable<PortableExecutableReference> references = new[]
            {
                MetadataReference.CreateFromFile(typeof(FilePath).Assembly.Location),
            };

            // Create a Roslyn compilation for the syntax tree.
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: references);

            // Create an instance of our incremental source generator
            var generator = new CustomAspectInterfaceGenerator();

            // The GeneratorDriver is used to run our generator against a compilation
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            // Run the source generator!
            driver = driver.RunGenerators(compilation);
            
            return driver.GetRunResult();
        }
    }
    
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
        await SourceGenerationTestHelper.VerifySerialization(source);
    }
}