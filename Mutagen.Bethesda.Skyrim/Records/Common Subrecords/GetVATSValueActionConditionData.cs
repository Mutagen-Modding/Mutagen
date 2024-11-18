namespace Mutagen.Bethesda.Skyrim;

partial class GetVATSValueActionConditionData
{
    public enum Action
    {
        UnarmedAttack,
        OneHandMeleeAttack,
        TwoHandMeleeAttack,
        MagicAttack,
        RangedAttack,
        Reload,
        Crouch,
        Stand,
        SwitchWeapon,
        ToggleWeaponDrawn,
        Heal,
        PlayerDeath,
    }

    public object? Parameter1
    {
        get => ValueFunction.VatsAction;
        set
        {

        }
    }
    public Type? Parameter1Type => typeof(ValueFunction);

    public object? Parameter2
    {
        get => null;
        set => Value = value is GetVATSValueActionConditionData.Action v ? v : throw new ArgumentException();
    }
    public Type? Parameter2Type => typeof(GetVATSValueActionConditionData.Action);
}