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

    public override object? Parameter1
    {
        get => ValueFunction.VatsAction;
        set
        {

        }
    }
    public override Type? Parameter1Type => typeof(ValueFunction);

    public override object? Parameter2
    {
        get => null;
        set => Value = value is GetVATSValueActionConditionData.Action v ? v : throw new ArgumentException();
    }
    public override Type? Parameter2Type => typeof(GetVATSValueActionConditionData.Action);
}