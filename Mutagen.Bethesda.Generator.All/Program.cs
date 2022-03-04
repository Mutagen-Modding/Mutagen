using Autofac;
using Loqui;
using Mutagen.Bethesda.Generation.Generator;
using Mutagen.Bethesda.Oblivion.Generator;
using Mutagen.Bethesda.Fallout4.Generator;
using Mutagen.Bethesda.Pex.Generator;
using Mutagen.Bethesda.Skyrim.Generator;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generator.All
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
            ContainerBuilder builder = new();
            builder.RegisterModule<GeneratorAutofacModule>();
            builder.RegisterAssemblyTypes(
                    typeof(Program).Assembly,
                    typeof(PexGenerationConstructor).Assembly,
                    typeof(OblivionGenerationConstructor).Assembly,
                    typeof(SkyrimGenerationConstructor).Assembly,
                    typeof(Fallout4GenerationConstructor).Assembly)
                .AsSelf()
                .AsImplementedInterfaces();
            var cont = builder.Build();
            var runner = cont.Resolve<GenerationRunner>();

            await runner.Generate();
        }
    }
}
