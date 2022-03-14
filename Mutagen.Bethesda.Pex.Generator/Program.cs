using Autofac;
using Mutagen.Bethesda.Generation.Generator;

ContainerBuilder builder = new();
builder.RegisterModule<GeneratorAutofacModule>();
builder.RegisterAssemblyTypes(typeof(Program).Assembly)
    .AsSelf()
    .AsImplementedInterfaces();
var cont = builder.Build();
var runner = cont.Resolve<GenerationRunner>();

await runner.Generate();