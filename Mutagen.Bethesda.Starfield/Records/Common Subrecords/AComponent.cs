using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Strings.DI;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

public partial class AComponent
{
    public enum ComponentType
    {
        BGSAnimationGraph_Component,
        BGSAttachParentArray_Component,
        BGSActivityTracker,
        BGSEffectSequenceComponent,
        BGSScannable,
        BGSKeywordForm_Component,
        BGSObjectWindowFilter_Component,
        BGSContactShadowComponent_Component,
        TESFullName_Component,
        TESModel_Component,
        TESPlanetModel_Component,
        HoudiniData_Component,
        BGSSkinForm_Component,
        BGSBodyPartInfo_Component,
        LightAttachmentFormComponent,
        LightAnimFormComponent,
        ParticleSystem_Component,
        BGSLodOwner_Component,
        BGSSoundTag_Component,
        BGSDisplayCase,
        BGSObjectPaletteDefaults_Component,
        Volumes_Component,
        BGSPlanetContentManagerContentProperties_Component,
    }

    public static bool TryCreateFromBinary(
        MutagenFrame frame,
        out AComponent item,
        TypedParseParams translationParams = default)
    {
        var startPos = frame.Position;
        item = CreateFromBinary(
            frame: frame,
            translationParams: translationParams);
        return startPos != frame.Position;
    }

    public static AComponent CreateFromBinary(
        MutagenFrame frame,
        TypedParseParams translationParams)
    {
        var bfcb = frame.GetSubrecord(RecordTypes.BFCB);
        var componentType = GetComponentType(bfcb);
        switch (componentType)
        {
            case ComponentType.BGSAnimationGraph_Component:
                return AnimationGraphComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSAttachParentArray_Component:
                return AttachParentArrayComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSActivityTracker:
                return ActivityTrackerComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSEffectSequenceComponent:
                return EffectSequenceComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSScannable:
                return ScannableComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSKeywordForm_Component:
                return KeywordFormComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSObjectWindowFilter_Component:
                return ObjectWindowFilterComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSContactShadowComponent_Component:
                return ContactShadowComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.TESFullName_Component:
                return FullNameComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.TESModel_Component:
                return ModelComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.TESPlanetModel_Component:
                return PlanetModelComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.HoudiniData_Component:
                return HoudiniDataComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSSkinForm_Component:
                return SkinFormComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSBodyPartInfo_Component: 
                return BodyPartInfoComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.LightAttachmentFormComponent:
                return LightAttachmentFormComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.LightAnimFormComponent:
                return LightAnimFormComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.ParticleSystem_Component:
                return ParticleSystemComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSLodOwner_Component:
                return LodOwnerComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSSoundTag_Component:
                return SoundTagComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSDisplayCase:
                return DisplayCaseComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSObjectPaletteDefaults_Component:
                return ObjectPaletteDefaultsComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.Volumes_Component:
                return VolumesComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSPlanetContentManagerContentProperties_Component:
                return PlanetContentManagerContentProperties.CreateFromBinary(frame, translationParams);
            default:
                throw new NotImplementedException();
        }
    }

    public static ComponentType GetComponentType(SubrecordFrame bfcb)
    {
        return Enum.Parse<ComponentType>(bfcb.AsString(MutagenEncoding._1252));
    }
}

partial class AComponentBinaryOverlay
{
    public partial ParseResult BFCBStringCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        stream.ReadSubrecord(RecordTypes.BFCB);
        return null;
    }
}

partial class AComponentBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryBFCBStringCustom(
        MutagenFrame frame,
        IAComponent item,
        PreviousParse lastParsed)
    {
        frame.ReadSubrecord(RecordTypes.BFCB);
        return null;
    }
}

partial class AComponentBinaryWriteTranslation
{
    public static partial void WriteBinaryBFCBStringCustom(
        MutagenWriter writer,
        IAComponentGetter item)
    {
        AComponent.ComponentType type = item switch
        {
            IAnimationGraphComponentGetter _ => AComponent.ComponentType.BGSAnimationGraph_Component,
            IAttachParentArrayComponentGetter _ => AComponent.ComponentType.BGSAttachParentArray_Component,
            IActivityTrackerComponentGetter _ => AComponent.ComponentType.BGSActivityTracker,
            IEffectSequenceComponentGetter _ => AComponent.ComponentType.BGSEffectSequenceComponent,
            IScannableComponentGetter _ => AComponent.ComponentType.BGSScannable,
            IKeywordFormComponentGetter _ => AComponent.ComponentType.BGSKeywordForm_Component,
            IObjectWindowFilterComponentGetter _ => AComponent.ComponentType.BGSObjectWindowFilter_Component,
            IContactShadowComponentGetter _ => AComponent.ComponentType.BGSContactShadowComponent_Component,
            IFullNameComponentGetter _ => AComponent.ComponentType.TESFullName_Component,
            IModelComponentGetter _ => AComponent.ComponentType.TESModel_Component,
            IPlanetModelComponentGetter _ => AComponent.ComponentType.TESPlanetModel_Component,
            IHoudiniDataComponentGetter _ => AComponent.ComponentType.HoudiniData_Component,
            ISkinFormComponentGetter _ => AComponent.ComponentType.BGSSkinForm_Component,
            IBodyPartInfoComponentGetter _ => AComponent.ComponentType.BGSBodyPartInfo_Component,
            ILightAttachmentFormComponentGetter _ => AComponent.ComponentType.LightAttachmentFormComponent,
            ILightAnimFormComponentGetter _ => AComponent.ComponentType.LightAnimFormComponent,
            IParticleSystemComponentGetter _ => AComponent.ComponentType.ParticleSystem_Component,
            ILodOwnerComponentGetter _ => AComponent.ComponentType.BGSLodOwner_Component,
            ISoundTagComponentGetter _ => AComponent.ComponentType.BGSSoundTag_Component,
            IDisplayCaseComponentGetter _ => AComponent.ComponentType.BGSDisplayCase,
            IObjectPaletteDefaultsComponentGetter _ => AComponent.ComponentType.BGSObjectPaletteDefaults_Component,
            IVolumesComponentGetter _ => AComponent.ComponentType.Volumes_Component,
            IPlanetContentManagerContentPropertiesGetter _ => AComponent.ComponentType.BGSPlanetContentManagerContentProperties_Component,
            _ => throw new NotImplementedException()
        };

        using (HeaderExport.Subrecord(writer, RecordTypes.BFCB))
        {
            writer.Write(type.ToStringFast(), StringBinaryType.NullTerminate, MutagenEncoding._1252);
        }
    }
}

partial class AComponentBinaryOverlay
{
    public static IAComponentGetter AComponentFactory(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        TypedParseParams translationParams)
    {
        var componentType = AComponent.GetComponentType(stream.GetSubrecord(RecordTypes.BFCB));
        switch (componentType)
        {
            case AComponent.ComponentType.BGSAnimationGraph_Component:
                return AnimationGraphComponentBinaryOverlay.AnimationGraphComponentFactory(stream, package);
            case AComponent.ComponentType.BGSAttachParentArray_Component:
                return AttachParentArrayComponentBinaryOverlay.AttachParentArrayComponentFactory(stream, package);
            case AComponent.ComponentType.BGSActivityTracker:
                return ActivityTrackerComponentBinaryOverlay.ActivityTrackerComponentFactory(stream, package);
            case AComponent.ComponentType.BGSEffectSequenceComponent:
                return EffectSequenceComponentBinaryOverlay.EffectSequenceComponentFactory(stream, package);
            case AComponent.ComponentType.BGSScannable:
                return ScannableComponentBinaryOverlay.ScannableComponentFactory(stream, package);
            case AComponent.ComponentType.BGSKeywordForm_Component:
                return KeywordFormComponentBinaryOverlay.KeywordFormComponentFactory(stream, package);
            case AComponent.ComponentType.BGSObjectWindowFilter_Component:
                return ObjectWindowFilterComponentBinaryOverlay.ObjectWindowFilterComponentFactory(stream, package);
            case AComponent.ComponentType.BGSContactShadowComponent_Component:
                return ContactShadowComponentBinaryOverlay.ContactShadowComponentFactory(stream, package);
            case AComponent.ComponentType.TESFullName_Component:
                return FullNameComponentBinaryOverlay.FullNameComponentFactory(stream, package);
            case AComponent.ComponentType.TESModel_Component:
                return ModelComponentBinaryOverlay.ModelComponentFactory(stream, package);
            case AComponent.ComponentType.TESPlanetModel_Component:
                return PlanetModelComponentBinaryOverlay.PlanetModelComponentFactory(stream, package);
            case AComponent.ComponentType.HoudiniData_Component:
                return HoudiniDataComponentBinaryOverlay.HoudiniDataComponentFactory(stream, package);
            case AComponent.ComponentType.BGSSkinForm_Component:
                return SkinFormComponentBinaryOverlay.SkinFormComponentFactory(stream, package);
            case AComponent.ComponentType.BGSBodyPartInfo_Component:
                return BodyPartInfoComponentBinaryOverlay.BodyPartInfoComponentFactory(stream, package);
            case AComponent.ComponentType.LightAttachmentFormComponent:
                return LightAttachmentFormComponentBinaryOverlay.LightAttachmentFormComponentFactory(stream, package);
            case AComponent.ComponentType.LightAnimFormComponent:
                return LightAnimFormComponentBinaryOverlay.LightAnimFormComponentFactory(stream, package);
            case AComponent.ComponentType.ParticleSystem_Component:
                return ParticleSystemComponentBinaryOverlay.ParticleSystemComponentFactory(stream, package);
            case AComponent.ComponentType.BGSLodOwner_Component:
                return LodOwnerComponentBinaryOverlay.LodOwnerComponentFactory(stream, package);
            case AComponent.ComponentType.BGSSoundTag_Component:
                return SoundTagComponentBinaryOverlay.SoundTagComponentFactory(stream, package);
            case AComponent.ComponentType.BGSDisplayCase:
                return DisplayCaseComponentBinaryOverlay.DisplayCaseComponentFactory(stream, package);
            case AComponent.ComponentType.BGSObjectPaletteDefaults_Component:
                return ObjectPaletteDefaultsComponentBinaryOverlay.ObjectPaletteDefaultsComponentFactory(stream, package);
            case AComponent.ComponentType.Volumes_Component:
                return VolumesComponentBinaryOverlay.VolumesComponentFactory(stream, package);
            case AComponent.ComponentType.BGSPlanetContentManagerContentProperties_Component:
                return PlanetContentManagerContentPropertiesBinaryOverlay.PlanetContentManagerContentPropertiesFactory(stream, package);
            default:
                throw new NotImplementedException();
        }
        
    }
}