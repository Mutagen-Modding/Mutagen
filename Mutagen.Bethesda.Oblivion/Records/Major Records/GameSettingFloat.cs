using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class GameSettingFloat
    {
        public const char TRIGGER_CHAR = 'f';
        protected override char TriggerChar => TRIGGER_CHAR;
    }
}
