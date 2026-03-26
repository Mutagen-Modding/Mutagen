using System.Diagnostics;
using Autofac;
using Mutagen.Bethesda.Generation.Generator;

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

try
{
    await runner.Generate();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"GENERATION FAILED: {ex.Message}");
    Console.Error.WriteLine($"FULL: {ex}");
    throw;
}