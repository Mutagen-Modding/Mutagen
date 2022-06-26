using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Skyrim;

public partial class GameSettingFloat : IGameSettingNumeric
{
    public override GameSettingType SettingType => GameSettingType.Float;

    float? IGameSettingNumeric.RawData
    {
        get => this.Data;
        set => this.Data = value;
    }
}