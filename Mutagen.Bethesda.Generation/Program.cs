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
            GenerateRecords();
            GenerateTester();
        }

        static void GenerateRecords()
        {
            LoquiGenerator gen = new LoquiGenerator()
            {
                RaisePropertyChangedDefault = false,
                NotifyingDefault = true,
                HasBeenSetDefault = true
            };
            gen.XmlTranslation.ShouldGenerateXSD = false;
            gen.XmlTranslation.AddTypeAssociation<FormIDLinkType>(new FormIDLinkXmlTranslationGeneration());
            gen.XmlTranslation.AddTypeAssociation<RawFormIDType>(new PrimitiveXmlTranslationGeneration<RawFormID>());
            gen.MaskModule.AddTypeAssociation<FormIDLinkType>(MaskModule.TypicalField);
            gen.GenerationModules.Add(new MutagenModule());
            gen.Add(new BinaryTranslationModule(gen));
            gen.AddTypeAssociation<FormIDLinkType>("FormID");
            gen.AddTypeAssociation<RawFormIDType>("RawFormID");
            gen.AddTypeAssociation<BufferType>("Buffer");
            gen.AddTypeAssociation<DataType>("Data");
            gen.AddTypeAssociation<ZeroType>("Zero");
            gen.AddTypeAssociation<TransferType>("Transfer");
            gen.AddTypeAssociation<GroupType>("Group");
            gen.AddTypeAssociation<SpecialParseType>("SpecialParse");
            gen.ReplaceTypeAssociation<Loqui.Generation.EnumType, Mutagen.Bethesda.Generation.EnumType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.StringType, Mutagen.Bethesda.Generation.StringType>();
            Loqui.Generation.Presentation.Utility.AddToLoquiGenerator(gen);

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

        static void GenerateTester()
        {
            LoquiGenerator gen = new LoquiGenerator()
            {
                RaisePropertyChangedDefault = false,
                NotifyingDefault = false,
                HasBeenSetDefault = false
            };
            var testerProto = gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("Tests"),
                    new DirectoryInfo("../../../Mutagen.Bethesda.Tests/Generated"))
                {
                    DefaultNamespace = "Mutagen.Bethesda.Tests",
                });
            testerProto.AddProjectToModify(
                new FileInfo("../../../Mutagen.Bethesda.Tests/Mutagen.Bethesda.Tests.csproj"));

            gen.Generate().Wait();
        }
    }
}
