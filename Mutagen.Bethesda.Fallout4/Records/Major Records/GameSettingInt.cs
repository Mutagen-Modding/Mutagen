using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class GameSettingInt : IGameSettingNumeric
    {
        public override GameSettingType SettingType => GameSettingType.Int;

        public float? RawData
        {
            get => this.Data;
            set => this.Data = value.HasValue ? (int)value.Value : default;
        }
    }
}
