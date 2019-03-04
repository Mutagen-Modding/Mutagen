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
            LoquiGenerator gen = new LoquiGenerator(typical: false)
            {
                RaisePropertyChangedDefault = false,
                NotifyingDefault = NotifyingType.ReactiveUI,
                ObjectCentralizedDefault = true,
                HasBeenSetDefault = true,
                ToStringDefault = false,
            };
            gen.AddTypicalTypeAssociations();
            gen.XmlTranslation = new MutagenXmlModule(gen);
            gen.Add(gen.XmlTranslation);
            gen.Add(gen.MaskModule);
            gen.Add(gen.ObjectCentralizationModule);
            gen.Add(new ReactiveModule());
            gen.Namespaces.Add("Mutagen.Bethesda.Internals");
            gen.XmlTranslation.ShouldGenerateXSD = false;
            gen.XmlTranslation.ExportWithIGetter = false;
            gen.XmlTranslation.AddTypeAssociation<FormIDLinkType>(new FormIDLinkXmlTranslationGeneration());
            gen.XmlTranslation.AddTypeAssociation<FormIDType>(new PrimitiveXmlTranslationGeneration<FormID>());
            gen.XmlTranslation.AddTypeAssociation<FormKeyType>(new PrimitiveXmlTranslationGeneration<FormKey>());
            gen.XmlTranslation.AddTypeAssociation<ModKeyType>(new PrimitiveXmlTranslationGeneration<ModKey>());
            gen.XmlTranslation.AddTypeAssociation<DataType>(new DataTypeXmlTranslationGeneration());
            gen.MaskModule.AddTypeAssociation<FormIDLinkType>(MaskModule.TypicalField);
            gen.GenerationModules.Add(new MutagenModule());
            gen.Add(new BinaryTranslationModule(gen));
            //gen.Add(new ObservableModModule());
            gen.AddTypeAssociation<FormIDLinkType>("FormIDLink");
            gen.AddTypeAssociation<FormIDType>("FormID");
            gen.AddTypeAssociation<FormKeyType>("FormKey");
            gen.AddTypeAssociation<ModKeyType>("ModKey");
            gen.AddTypeAssociation<BufferType>("Buffer");
            gen.AddTypeAssociation<DataType>("Data");
            gen.AddTypeAssociation<ZeroType>("Zero");
            gen.AddTypeAssociation<CustomLogic>("CustomLogic");
            gen.AddTypeAssociation<TransferType>("Transfer");
            gen.AddTypeAssociation<GroupType>("Group");
            gen.AddTypeAssociation<SpecialParseType>("SpecialParse");
            gen.ReplaceTypeAssociation<Loqui.Generation.EnumType, Mutagen.Bethesda.Generation.EnumType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.StringType, Mutagen.Bethesda.Generation.StringType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.LoquiType, Mutagen.Bethesda.Generation.MutagenLoquiType>();
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
                NotifyingDefault = NotifyingType.None,
                ObjectCentralizedDefault = false,
                HasBeenSetDefault = false
            };
            gen.XmlTranslation.ShouldGenerateXSD = true;
            var testerProto = gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("Tests"),
                    new DirectoryInfo("../../../Mutagen.Bethesda.Tests/Settings"))
                {
                    DefaultNamespace = "Mutagen.Bethesda.Tests",
                });
            testerProto.AddProjectToModify(
                new FileInfo("../../../Mutagen.Bethesda.Tests/Mutagen.Bethesda.Tests.csproj"));

            gen.Generate().Wait();
        }
    }
}
