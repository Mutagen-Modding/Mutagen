using Autofac;
using Mutagen.Bethesda.Generation.Generator;
using Mutagen.Bethesda.Oblivion.Generator;
using Mutagen.Bethesda.Fallout4.Generator;
using Mutagen.Bethesda.Pex.Generator;
using Mutagen.Bethesda.Skyrim.Generator;
using System;
using System.Diagnostics;

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

#if DEBUG
var detector = cont.Resolve<GenerationLineDetector>();
detector.LineDetected.Subscribe(x =>
{
    Debugger.Break();
});
#endif

await runner.Generate();
