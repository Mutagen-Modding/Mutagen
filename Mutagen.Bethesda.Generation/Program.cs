using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    class Program
    {
        static void Main(string[] args)
        {
            LoquiGenerator gen = new LoquiGenerator()
            {
                RaisePropertyChangedDefault = false,
                NotifyingDefault = true,
                HasBeenSetDefault = true
            };
            gen.XmlTranslation.ShouldGenerateXSD = false;
            gen.XmlTranslation.AddTypeAssociation<FormIDType>(new PrimitiveXmlTranslationGeneration<FormID>());
            gen.GenerationModules.Add(new MutagenModule());
            gen.Add(new BinaryTranslationModule(gen));
            gen.AddTypeAssociation<FormIDType>("FormID");
            gen.AddTypeAssociation<BufferType>("Buffer");
            gen.AddTypeAssociation<DataType>("Data");
            gen.ReplaceTypeAssociation<Loqui.Generation.EnumType, Mutagen.Bethesda.Generation.EnumType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.StringType, Mutagen.Bethesda.Generation.StringType>();

            var bethesdaProto = gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("Bethesda"),
                    new DirectoryInfo("../../../Mutagen.Bethesda"))
                {
                    DefaultNamespace = "Mutagen.Bethesda",
                });
            bethesdaProto.AddProjectToModify(
                new FileInfo(Path.Combine(bethesdaProto.GenerationFolder.FullName, "Mutagen.Bethesda.csproj")));

            var oblivProto = gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("Oblivion"),
                    new DirectoryInfo("../../../Mutagen.Bethesda.Oblivion"))
                {
                    DefaultNamespace = "Mutagen.Bethesda.Oblivion",
                });
            oblivProto.AddProjectToModify(
                new FileInfo(Path.Combine(oblivProto.GenerationFolder.FullName, "Mutagen.Bethesda.Oblivion.csproj")));

            gen.Generate().Wait();
        }
    }
}
