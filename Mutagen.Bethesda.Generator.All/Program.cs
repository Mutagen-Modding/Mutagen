using Autofac;
using Mutagen.Bethesda.Generation.Generator;
using Mutagen.Bethesda.Oblivion.Generator;
using Mutagen.Bethesda.Fallout4.Generator;
using Mutagen.Bethesda.Pex.Generator;
using Mutagen.Bethesda.Skyrim.Generator;
using System.Diagnostics;
using Mutagen.Bethesda.Starfield.Generator;
using Mutagen.Bethesda.Fallout3.Generator;
using Mutagen.Bethesda.FalloutNV.Generator;

ContainerBuilder builder = new();
builder.RegisterModule<GeneratorAutofacModule>();
builder.RegisterAssemblyTypes(
        typeof(Program).Assembly,
        typeof(PexGenerationConstructor).Assembly,
        typeof(OblivionGenerationConstructor).Assembly,
        typeof(SkyrimGenerationConstructor).Assembly,
        typeof(Fallout4GenerationConstructor).Assembly,
        typeof(StarfieldGenerationConstructor).Assembly,
        typeof(Fallout3GenerationConstructor).Assembly,
        typeof(FalloutNVGenerationConstructor).Assembly)
    .AsSelf()
    .AsImplementedInterfaces();
var cont = builder.Build();
var runner = cont.Resolve<GenerationRunner>();

#if DEBUG
var detector = cont.Resolve<GenerationLineDetector>();
detector.LineDetected.Subscribe(x =>
{
    Debugger.Break();
});
#endif

await runner.Generate();
