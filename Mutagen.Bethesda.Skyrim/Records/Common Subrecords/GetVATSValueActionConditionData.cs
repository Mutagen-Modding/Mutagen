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

    object? IConditionParameters.Parameter1
    {
        get => ValueFunction.VatsAction;
        set
        {

        }
    }
    Type? IConditionParametersGetter.Parameter1Type => typeof(ValueFunction);

    object? IConditionParameters.Parameter2
    {
        get => null;
        set => Value = value is GetVATSValueActionConditionData.Action v ? v : throw new ArgumentException();
    }
    Type? IConditionParametersGetter.Parameter2Type => typeof(GetVATSValueActionConditionData.Action);
}