using Mutagen.Bethesda.Internals;
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
                config.SetMove(new FileSection(move.SectionToMove), new FileLocation(move.LocationToMove));
            }
            foreach (var sub in this.Substitutions)
            {
                config.SetSubstitution(new FileLocation(sub.Location), sub.Data);
            }
            foreach (var add in this.Additions)
            {
                config.SetAddition(new FileLocation(add.Location), add.Data);
            }
            return config;
        }
    }
}
