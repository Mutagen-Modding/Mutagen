using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            LoquiGenerator gen = new LoquiGenerator(
                new DirectoryInfo("../../../Mutagen"))
            {
                DefaultNamespace = "Mutagen",
                RaisePropertyChangedDefault = false,
                ProtocolDefault = new ProtocolKey("Mutagen"),
                NotifyingDefault = NotifyingOption.Notifying
            };
            gen.XmlTranslation.ShouldGenerateXSD = false;
            gen.XmlTranslation.AddTypeAssociation<FormIDType>(new PrimitiveXmlTranslationGeneration<FormID>());
            gen.GenerationModules.Add(new MutagenModule());
            gen.Add(new BinaryTranslationModule(gen));
            gen.AddTypeAssociation<FormIDType>("FormID");
            gen.AddTypeAssociation<ZeroBufferType>("ZeroBuffer");
            gen.ReplaceTypeAssociation<Loqui.Generation.EnumType, Mutagen.Generation.EnumType>();

            // Add Projects
            gen.AddProjectToModify(
                new FileInfo(Path.Combine(gen.CommonGenerationFolder.FullName, "Mutagen.csproj")));

            gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("Mutagen")));

            await gen.Generate();
        }
    }
}
