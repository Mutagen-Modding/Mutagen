using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda.Generation.Fields;
using BoolType = Mutagen.Bethesda.Generation.Fields.BoolType;
using DictType = Mutagen.Bethesda.Generation.Fields.DictType;
using EnumType = Mutagen.Bethesda.Generation.Fields.EnumType;
using FloatType = Mutagen.Bethesda.Generation.Fields.FloatType;
using PercentType = Mutagen.Bethesda.Generation.Fields.PercentType;
using StringType = Mutagen.Bethesda.Generation.Fields.StringType;
using Mutagen.Bethesda.Generation.Generator;

namespace Mutagen.Bethesda.Generation
{
    class Program
    {
        public static string[] Args;

        static void AttachDebugInspector()
        {
            string testString = "StringBinaryTranslation.Instance.Write(writer, param1, StringBinaryType.NullTerminate, writer.MetaData.Encodings.NonTranslated);";
            FileGeneration.LineAppended
                .Where(i => i.Contains(testString))
                .Subscribe(s =>
                {
                    int wer = 23;
                    wer++;
                });
        }

        static async Task Main(string[] args)
        {
            Args = args;
#if DEBUG
            AttachDebugInspector();
#endif
            await GenerateRecords();
            GeneratePex();
        }
 
        static bool ShouldRun(string key)
        {
            if (Args.Length == 0) return true;
            return Args.Contains(key, StringComparer.OrdinalIgnoreCase);
        }

        static async Task GenerateRecords()
        {
            var adder = new TypicalRecordProtocolAdder();
            LoquiGenerator gen = new LoquiGenerator(typical: false)
            {
                NotifyingDefault = NotifyingType.None,
                NullableDefault = true,
                ToStringDefault = false,
            };
            gen.AddTypicalTypeAssociations();
            gen.Add(gen.MaskModule);
            gen.Namespaces.Add("Mutagen.Bethesda.Internals");
            gen.MaskModule.AddTypeAssociation<FormLinkType>(MaskModule.TypicalField);
            gen.MaskModule.AddTypeAssociation<GenderedType>(new GenderedItemMaskGeneration());
            gen.GenerationModules.Add(new PluginModule());
            gen.Add(new PluginTranslationModule(gen));
            gen.AddTypeAssociation<FormLinkType>("FormLink");
            gen.AddTypeAssociation<FormIDType>("FormID");
            gen.AddTypeAssociation<FormKeyType>("FormKey");
            gen.AddTypeAssociation<ModKeyType>("ModKey");
            gen.AddTypeAssociation<RecordTypeType>("RecordType");
            gen.AddTypeAssociation<BufferType>("Buffer");
            gen.AddTypeAssociation<DataType>("Data");
            gen.AddTypeAssociation<ZeroType>("Zero");
            gen.AddTypeAssociation<CustomLogic>("CustomLogic");
            gen.AddTypeAssociation<GroupType>("Group");
            gen.AddTypeAssociation<GenderedType>("Gendered");
            gen.AddTypeAssociation<BreakType>("Break");
            gen.AddTypeAssociation<MarkerType>("Marker");
            gen.ReplaceTypeAssociation<Loqui.Generation.EnumType, EnumType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.StringType, StringType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.LoquiType, MutagenLoquiType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.FloatType, FloatType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.PercentType, PercentType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.DictType, DictType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.BoolType, BoolType>();

            // Always run core
            var bethesdaProto = gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("Bethesda"),
                    new DirectoryInfo("../../../../Mutagen.Bethesda.Core/Plugins/Records"))
                {
                    DefaultNamespace = "Mutagen.Bethesda.Plugins.Records",
                    DoGeneration = ShouldRun("All")
                });
            bethesdaProto.AddProjectToModify(
                new FileInfo(Path.Combine(bethesdaProto.GenerationFolder.FullName, "../../Mutagen.Bethesda.Core.csproj")));

            if (ShouldRun("All"))
            {
                var proto = gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("All"),
                    new DirectoryInfo("../../../../Mutagen.Bethesda/Plugins/Records"))
                {
                    DefaultNamespace = "Mutagen.Bethesda",
                });
                proto.AddProjectToModify(
                    new FileInfo(Path.Combine(proto.GenerationFolder.FullName, "../../Mutagen.Bethesda.csproj")));
            }

            if (ShouldRun("Oblivion"))
            {
                adder.Add(gen, "Oblivion");
            }

            if (ShouldRun("Skyrim"))
            {
                adder.Add(gen, "Skyrim");
            }

            if (ShouldRun("Fallout4"))
            {
                adder.Add(gen, "Fallout4");
            }

            await gen.Generate();
        }

        static void GeneratePex()
        {
            LoquiGenerator gen = new LoquiGenerator(typical: false)
            {
                NotifyingDefault = NotifyingType.None,
                NullableDefault = false,
                ToStringDefault = false,
            };
            gen.AddTypicalTypeAssociations();
            gen.Add(gen.MaskModule);

            var dir = new DirectoryInfo("../../../../Mutagen.Bethesda.Core/Pex/DataTypes");
            var pexProto = gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("Pex"),
                    dir)
                {
                    DefaultNamespace = "Mutagen.Bethesda.Pex",
                    DoGeneration = ShouldRun("Pex")
                });
            var projFile = new FileInfo(Path.Combine(pexProto.GenerationFolder.FullName, "../../Mutagen.Bethesda.Core.csproj"));
            pexProto.AddProjectToModify(projFile);

            gen.Generate().Wait();
        }
    }
}
