using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BethesdaProc.Generation
{
    class Program
    {
        static void Main(string[] args)
        {
            LoquiGenerator gen = new LoquiGenerator(
                new DirectoryInfo("../../../BethesdaProc"))
            {
                DefaultNamespace = "BethesdaProc",
                RaisePropertyChangedDefault = false,
                ProtocolDefault = new ProtocolKey("BethesdaProc")
            };

            // Add Projects
            gen.AddProjectToModify(
                new FileInfo(Path.Combine(gen.CommonGenerationFolder.FullName, "BethesdaProc.csproj")));

            gen.AddProtocol(
                new ProtocolGeneration(
                    gen,
                    gen.ProtocolDefault));

            gen.Generate();
        }
    }
}
