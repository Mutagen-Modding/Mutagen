using Loqui;
using Loqui.Generation;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
            var provider = new RecordGeneratorProvider();
            LoquiGenerator gen = provider.Generator;

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
