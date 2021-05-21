using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Oblivion
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
