using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;

namespace Mutagen.Generation
{
    public class OblivionBinaryTranslationModule : BinaryTranslationModule
    {
        public override string ModuleNickname => "OblivionBinary";

        public OblivionBinaryTranslationModule(LoquiGenerator gen)
            : base(gen)
        {
        }
    }
}
