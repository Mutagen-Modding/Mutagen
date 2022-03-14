using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Generator;

namespace Mutagen.Bethesda.Pex.Generator;

public class PexGenerationConstructor : IGenerationConstructor
{
    public LoquiGenerator Construct()
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
                DefaultNamespace = "Mutagen.Bethesda.Pex"
            });
        var projFile = new FileInfo(Path.Combine(pexProto.GenerationFolder.FullName, "../../Mutagen.Bethesda.Core.csproj"));
        pexProto.AddProjectToModify(projFile);
        return gen;
    }
}
