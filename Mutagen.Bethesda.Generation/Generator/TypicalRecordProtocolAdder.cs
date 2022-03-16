using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Generator;

public class TypicalRecordProtocolAdder
{
    public void Add(LoquiGenerator gen, string name)
    {
        var proto = gen.AddProtocol(
        new ProtocolGeneration(
            gen,
            new ProtocolKey(name),
            new DirectoryInfo($"../../../../Mutagen.Bethesda.{name}/Records"))
        {
            DefaultNamespace = $"Mutagen.Bethesda.{name}",
        });
        proto.AddProjectToModify(
            new FileInfo(Path.Combine(proto.GenerationFolder.FullName, $"../Mutagen.Bethesda.{name}.csproj")));
    }
}
