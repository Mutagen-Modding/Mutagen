using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public partial class Instruction
    {
        public BinaryFileProcessor.Config ToProcessorConfig()
        {
            var config = new BinaryFileProcessor.Config();
            foreach (var move in this.Moves)
            {
                config.SetMove(move.SectionToMove, move.LocationToMove);
            }
            foreach (var sub in this.Substitutions)
            {
                config.SetSubstitution(sub.Location, sub.Data);
            }
            foreach (var add in this.Additions)
            {
                config.SetAddition(add.Location, add.Data);
            }
            return config;
        }
    }
}
