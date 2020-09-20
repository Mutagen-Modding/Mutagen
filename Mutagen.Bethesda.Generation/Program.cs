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
        public static string[] Args;

        static void AttachDebugInspector()
        {
            string testString = "SkyrimModBinaryTranslationMixIn";
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
            Args = args;
#if DEBUG
            AttachDebugInspector();
#endif
            GenerateRecords();
        }
 
        static bool ShouldRun(string key)
        {
            if (Args.Length == 0) return true;
            return Args.Contains(key, StringComparer.OrdinalIgnoreCase);
        }

        static void GenerateRecords()
        { 
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
            gen.GenerationModules.Add(new MutagenModule());
            gen.Add(new BinaryTranslationModule(gen));
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
            gen.ReplaceTypeAssociation<Loqui.Generation.EnumType, Mutagen.Bethesda.Generation.EnumType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.StringType, Mutagen.Bethesda.Generation.StringType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.LoquiType, Mutagen.Bethesda.Generation.MutagenLoquiType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.FloatType, Mutagen.Bethesda.Generation.FloatType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.PercentType, Mutagen.Bethesda.Generation.PercentType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.DictType, Mutagen.Bethesda.Generation.DictType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.BoolType, Mutagen.Bethesda.Generation.BoolType>();

            // Always run core
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

            if (ShouldRun("All"))
            {
                var proto = gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("All"),
                    new DirectoryInfo("../../../../Mutagen.Bethesda/Records"))
                {
                    DefaultNamespace = "Mutagen.Bethesda",
                });
                proto.AddProjectToModify(
                    new FileInfo(Path.Combine(proto.GenerationFolder.FullName, "../Mutagen.Bethesda.csproj")));
            }

            if (ShouldRun("Oblivion"))
            {
                var proto = gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("Oblivion"),
                    new DirectoryInfo("../../../../Mutagen.Bethesda.Oblivion/Records"))
                {
                    DefaultNamespace = "Mutagen.Bethesda.Oblivion",
                });
                proto.AddProjectToModify(
                    new FileInfo(Path.Combine(proto.GenerationFolder.FullName, "../Mutagen.Bethesda.Oblivion.csproj")));
            }

            if (ShouldRun("Skyrim"))
            {
                var proto = gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("Skyrim"),
                    new DirectoryInfo("../../../../Mutagen.Bethesda.Skyrim/Records"))
                {
                    DefaultNamespace = "Mutagen.Bethesda.Skyrim",
                });
                proto.AddProjectToModify(
                    new FileInfo(Path.Combine(proto.GenerationFolder.FullName, "../Mutagen.Bethesda.Skyrim.csproj")));
            }

            gen.Generate().Wait();
        }
    }
}
