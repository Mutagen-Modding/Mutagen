using Autofac;
using Mutagen.Bethesda.Generation.Generator;
using System.Diagnostics;

ContainerBuilder builder = new();
builder.RegisterModule<GeneratorAutofacModule>();
builder.RegisterAssemblyTypes(typeof(Program).Assembly)
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