using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class GameSettingFloat : IGameSettingNumeric
    {
        public override GameSettingType SettingType => GameSettingType.Float;

        float? IGameSettingNumeric.RawData
        {
            get => this.Data;
            set => this.Data = value;
        }
    }
}
