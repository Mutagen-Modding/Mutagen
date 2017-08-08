using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    class Program
    {
        static void Main(string[] args)
        {
            LoquiGenerator gen = new LoquiGenerator(
                new DirectoryInfo("../../../Mutagen"))
            {
                DefaultNamespace = "Mutagen",
                RaisePropertyChangedDefault = false,
                ProtocolDefault = new ProtocolKey("Mutagen"),
                NotifyingDefault = NotifyingOption.HasBeenSet
            };
            gen.Add(new OblivionBinaryTranslationModule(gen));
            gen.AddTypeAssociation<UnknownType>("Unknown");

            // Add Projects
            gen.AddProjectToModify(
                new FileInfo(Path.Combine(gen.CommonGenerationFolder.FullName, "Mutagen.csproj")));

            gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    new ProtocolKey("Mutagen")));

            gen.Generate();
        }
    }
}
