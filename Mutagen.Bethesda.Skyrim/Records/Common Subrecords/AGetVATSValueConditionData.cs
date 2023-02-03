using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Skyrim;

partial class AGetVATSValueConditionData : IConditionStringParameter
{
    string? IConditionStringParameterGetter.FirstStringParameter => FirstUnusedStringParameter;

    string? IConditionStringParameterGetter.SecondStringParameter => SecondUnusedStringParameter;

    string? IConditionStringParameter.FirstStringParameter
    {
        get => FirstUnusedStringParameter;
        set => FirstUnusedStringParameter = value;
    }

    string? IConditionStringParameter.SecondStringParameter
    {
        get => SecondUnusedStringParameter;
        set => SecondUnusedStringParameter = value;
    }

    Condition.Function IConditionDataGetter.Function => Condition.Function.GetVATSValue;

    public enum ValueFunction
    {
        WeaponIs,
        WeaponOrList,
        TargetIs,
        TargetOrList,
        TargetDistance,
        TargetPart,
        VatsAction,
        IsSuccess,
        IsCritical,
        CriticalEffectIs,
        CriticalEffectOrList,
        IsFatal,
        ExplodePart,
        DismemberPart,
        CripplePart,
        WeaponTypeIs,
        IsStranger,
        IsParalyzingPalm,
        ProjectileTypeIs,
        DeliveryTypeIs,
        CastingTypeIs,
    }

    public static AGetVATSValueConditionData CreateFromBinary(MutagenFrame frame)
    {
        var e = (ValueFunction)frame.ReadInt32();
        switch (e)
        {
            case ValueFunction.WeaponIs:
                return GetVATSValueWeaponConditionData.CreateFromBinary(frame);
            case ValueFunction.WeaponOrList:
                return GetVATSValueWeaponOrListConditionData.CreateFromBinary(frame);
            case ValueFunction.TargetIs:
                return GetVATSValueTargetConditionData.CreateFromBinary(frame);
            case ValueFunction.TargetOrList:
                return GetVATSValueTargetOrListConditionData.CreateFromBinary(frame);
            case ValueFunction.TargetDistance:
                return GetVATSValueTargetDistanceConditionData.CreateFromBinary(frame);
            case ValueFunction.TargetPart:
                return GetVATSValueTargetPartConditionData.CreateFromBinary(frame);
            case ValueFunction.VatsAction:
                return GetVATSValueActionConditionData.CreateFromBinary(frame);
            case ValueFunction.IsSuccess:
                return GetVATSValueIsSuccessConditionData.CreateFromBinary(frame);
            case ValueFunction.IsCritical:
                return GetVATSValueIsCriticalConditionData.CreateFromBinary(frame);
            case ValueFunction.CriticalEffectIs:
                return GetVATSValueCriticalEffectConditionData.CreateFromBinary(frame);
            case ValueFunction.CriticalEffectOrList:
                return GetVATSValueCriticalEffectOrListConditionData.CreateFromBinary(frame);
            case ValueFunction.IsFatal:
                return GetVATSValueIsFatalConditionData.CreateFromBinary(frame);
            case ValueFunction.ExplodePart:
                return GetVATSValueExplodePartConditionData.CreateFromBinary(frame);
            case ValueFunction.DismemberPart:
                return GetVATSValueDismemberPartConditionData.CreateFromBinary(frame);
            case ValueFunction.CripplePart:
                return GetVATSValueCripplePartConditionData.CreateFromBinary(frame);
            case ValueFunction.WeaponTypeIs:
                return GetVATSValueWeaponTypeConditionData.CreateFromBinary(frame);
            case ValueFunction.IsStranger:
                return GetVATSValueIsStrangerConditionData.CreateFromBinary(frame);
            case ValueFunction.IsParalyzingPalm:
                return GetVATSValueIsParalyzingPalmConditionData.CreateFromBinary(frame);
            case ValueFunction.ProjectileTypeIs:
                return GetVATSValueProjectileTypeConditionData.CreateFromBinary(frame);
            case ValueFunction.DeliveryTypeIs:
                return GetVATSValueDeliveryTypeConditionData.CreateFromBinary(frame);
            case ValueFunction.CastingTypeIs:
                return GetVATSValueCastingTypeConditionData.CreateFromBinary(frame);
            default:
                frame.Position -= 4;
                return GetVATSValueUnknownConditionData.CreateFromBinary(frame);
        }
    }

    public static ValueFunction GetValueFunction(IAGetVATSValueConditionDataGetter obj)
    {
        return obj switch
        {
            IGetVATSValueWeaponConditionDataGetter => ValueFunction.WeaponIs,
            IGetVATSValueWeaponOrListConditionDataGetter => ValueFunction.WeaponOrList,
            IGetVATSValueTargetConditionDataGetter => ValueFunction.TargetIs,
            IGetVATSValueTargetOrListConditionDataGetter => ValueFunction.TargetOrList,
            IGetVATSValueTargetDistanceConditionDataGetter => ValueFunction.TargetDistance,
            IGetVATSValueTargetPartConditionDataGetter => ValueFunction.TargetPart,
            IGetVATSValueActionConditionDataGetter => ValueFunction.VatsAction,
            IGetVATSValueIsSuccessConditionDataGetter => ValueFunction.IsSuccess,
            IGetVATSValueIsCriticalConditionDataGetter => ValueFunction.IsCritical,
            IGetVATSValueCriticalEffectConditionDataGetter => ValueFunction.CriticalEffectIs,
            IGetVATSValueCriticalEffectOrListConditionDataGetter => ValueFunction.CriticalEffectOrList,
            IGetVATSValueIsFatalConditionDataGetter => ValueFunction.IsFatal,
            IGetVATSValueExplodePartConditionDataGetter => ValueFunction.ExplodePart,
            IGetVATSValueDismemberPartConditionDataGetter => ValueFunction.DismemberPart,
            IGetVATSValueCripplePartConditionDataGetter => ValueFunction.CripplePart,
            IGetVATSValueWeaponTypeConditionDataGetter => ValueFunction.WeaponTypeIs,
            IGetVATSValueIsStrangerConditionDataGetter => ValueFunction.IsStranger,
            IGetVATSValueIsParalyzingPalmConditionDataGetter => ValueFunction.IsParalyzingPalm,
            IGetVATSValueProjectileTypeConditionDataGetter => ValueFunction.ProjectileTypeIs,
            IGetVATSValueDeliveryTypeConditionDataGetter => ValueFunction.DeliveryTypeIs,
            IGetVATSValueCastingTypeConditionDataGetter => ValueFunction.CastingTypeIs,
            _ => throw new NotImplementedException()
        };
    }
}

partial class AGetVATSValueConditionDataBinaryCreateTranslation
{
    public static partial void FillBinaryValueFunctionParseCustom(
        MutagenFrame frame,
        IAGetVATSValueConditionData item)
    {
    }
}

partial class AGetVATSValueConditionDataBinaryWriteTranslation
{
    public static partial void WriteBinaryValueFunctionParseCustom(
        MutagenWriter writer,
        IAGetVATSValueConditionDataGetter item)
    {
        writer.Write((int)AGetVATSValueConditionData.GetValueFunction(item));
    }
}