using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    class Program
    {
        static void AttachDebugInspector()
        {
            string testString = "em.Color = Mutagen.Bethesda.Binary.ColorBinaryTranslation.Instance.Parse(frame: frame);";
            FileGeneration.LineAppended
                .Where(i => i.Contains(testString))
                .Subscribe(s =>
                {
                    int wer = 23;
                    wer++;
                });
        }

        static void Main(string[] args)
        {
#if DEBUG
            AttachDebugInspector();
#endif
            GenerateRecords();
            GenerateTester();
        }
         
        static void GenerateRecords()
        { 
            LoquiGenerator gen = new LoquiGenerator(typical: false)
            {
                NotifyingDefault = NotifyingType.None,
                HasBeenSetDefault = true,
                ToStringDefault = false,
            };
            gen.AddTypicalTypeAssociations();
            var xmlGen = new MutagenXmlModule(gen);
            gen.Add(xmlGen);
            gen.Add(gen.MaskModule);
            gen.Namespaces.Add("Mutagen.Bethesda.Internals");
            xmlGen.ShouldGenerateXSD = false;
            xmlGen.AddTypeAssociation<FormLinkType>(new FormLinkXmlTranslationGeneration());
            xmlGen.AddTypeAssociation<FormIDType>(new PrimitiveXmlTranslationGeneration<FormID>());
            xmlGen.AddTypeAssociation<FormKeyType>(new PrimitiveXmlTranslationGeneration<FormKey>());
            xmlGen.AddTypeAssociation<ModKeyType>(new PrimitiveXmlTranslationGeneration<ModKey>());
            xmlGen.AddTypeAssociation<GenderedType>(new GenderedTypeXmlTranslationGeneration());
            gen.MaskModule.AddTypeAssociation<FormLinkType>(MaskModule.TypicalField);
            gen.MaskModule.AddTypeAssociation<GenderedType>(new GenderedItemMaskGeneration());
            gen.GenerationModules.Add(new MutagenModule());
            gen.Add(new BinaryTranslationModule(gen));
            gen.AddTypeAssociation<FormLinkType>("FormLink");
            gen.AddTypeAssociation<FormIDType>("FormID");
            gen.AddTypeAssociation<FormKeyType>("FormKey");
            gen.AddTypeAssociation<ModKeyType>("ModKey");
            gen.AddTypeAssociation<BufferType>("Buffer");
            gen.AddTypeAssociation<ZeroType>("Zero");
            gen.AddTypeAssociation<CustomLogic>("CustomLogic");
            gen.AddTypeAssociation<TransferType>("Transfer");
            gen.AddTypeAssociation<GroupType>("Group");
            gen.AddTypeAssociation<SpecialParseType>("SpecialParse");
            gen.AddTypeAssociation<GenderedType>("Gendered");
            gen.AddTypeAssociation<BreakType>("Break");
            gen.ReplaceTypeAssociation<Loqui.Generation.EnumType, Mutagen.Bethesda.Generation.EnumType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.StringType, Mutagen.Bethesda.Generation.StringType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.LoquiType, Mutagen.Bethesda.Generation.MutagenLoquiType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.FloatType, Mutagen.Bethesda.Generation.FloatType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.DictType, Mutagen.Bethesda.Generation.DictType>();

            var bethesdaProto = gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("Bethesda"),
                    new DirectoryInfo("../../../../Mutagen.Bethesda.Core/Records"))
                {
                    DefaultNamespace = "Mutagen.Bethesda",
                });
            bethesdaProto.AddProjectToModify(
                new FileInfo(Path.Combine(bethesdaProto.GenerationFolder.FullName, "../Mutagen.Bethesda.Core.csproj")));

            var oblivProto = gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("Oblivion"),
                    new DirectoryInfo("../../../../Mutagen.Bethesda/Oblivion"))
                {
                    DefaultNamespace = "Mutagen.Bethesda.Oblivion",
                });
            oblivProto.AddProjectToModify(
                new FileInfo(Path.Combine(oblivProto.GenerationFolder.FullName, "../Mutagen.Bethesda.csproj")));

            var skyrimProto = gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("Skyrim"),
                    new DirectoryInfo("../../../../Mutagen.Bethesda/Skyrim"))
                {
                    DefaultNamespace = "Mutagen.Bethesda.Skyrim",
                });
            skyrimProto.AddProjectToModify(
                new FileInfo(Path.Combine(skyrimProto.GenerationFolder.FullName, "../Mutagen.Bethesda.csproj")));

            gen.Generate().Wait();
        }

        static void GenerateTester()
        {
            LoquiGenerator gen = new LoquiGenerator()
            {
                NotifyingDefault = NotifyingType.None,
                HasBeenSetDefault = false
            };
            var xmlModule = new XmlTranslationModule(gen)
            {
                ShouldGenerateXSD = true
            };
            gen.Add(xmlModule);
            var testerProto = gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("Tests"),
                    new DirectoryInfo("../../../../Mutagen.Bethesda.Tests/Settings"))
                {
                    DefaultNamespace = "Mutagen.Bethesda.Tests",
                });
            testerProto.AddProjectToModify(
                new FileInfo("../../../../Mutagen.Bethesda.Tests/Mutagen.Bethesda.Tests.csproj"));

            gen.Generate().Wait();
        }
    }
}
