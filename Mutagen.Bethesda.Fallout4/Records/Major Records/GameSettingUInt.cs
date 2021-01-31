using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class GameSettingUInt : IGameSettingNumeric
    {
        public override GameSettingType SettingType => GameSettingType.UInt;

        public float? RawData
        {
            get => this.Data;
            set => this.Data = value.HasValue ? (uint)value.Value : default;
        }
    }
}
