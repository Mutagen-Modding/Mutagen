using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Oblivion
{
    public partial class GameSettingString
    {
        public const char TRIGGER_CHAR = 's';
        protected override char TriggerChar => TRIGGER_CHAR;
    }
}
