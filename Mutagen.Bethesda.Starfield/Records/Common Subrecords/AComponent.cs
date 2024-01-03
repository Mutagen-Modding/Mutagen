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
        BGSShipManagement,
        BGSExternalComponentDataSource_Component,
        BGSForcedLocRefType_Component,
        BGSLinkedVoiceType_Component,
        BGSPapyrusScripts_Component,
        BGSPathingData_Component,
        BGSPropertySheet_Component,
        BGSSpaceshipAIActor_Component,
        BGSSpaceshipEquipment_Component,
        BGSSpaceshipWeaponBindings_Component,
        BGSFormLinkData_Component,
        Blueprint_Component,
        BGSDestructibleObject_Component,
        TESContainer_Component,
        TESMagicTargetForm_Component,
        BGSStoredTraversals_Component,
        BGSAddToInventoryOnDestroy_Component,
        BGSCrowdComponent_Component,
        BGSSpaceshipHullCode_Component,
        BGSPrimitive_Component,
        BGSSpawnOnDestroy_Component,
        BGSWorldSpaceOverlay_Component,
        ReflectionProbes_Component,
        TESImageSpaceModifiableForm_Component,
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
                return PlanetContentManagerContentPropertiesComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSShipManagement:
                return ShipManagementComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSExternalComponentDataSource_Component:
                return ExternalDataSourceComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSForcedLocRefType_Component:
                return ForcedLocRefTypeComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSLinkedVoiceType_Component:
                return LinkedVoiceTypeComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSPapyrusScripts_Component:
                return PapyrusScriptsComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSPathingData_Component:
                return PathingDataComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSPropertySheet_Component:
                return PropertySheetComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSSpaceshipAIActor_Component:
                return SpaceshipAIActorComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSSpaceshipEquipment_Component:
                return SpaceshipEquipmentComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSSpaceshipWeaponBindings_Component:
                return SpaceshipWeaponBindingsComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSFormLinkData_Component:
                return FormLinkDataComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.Blueprint_Component:
                return BlueprintComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSDestructibleObject_Component:
                return DestructibleObjectComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.TESContainer_Component:
                return ContainerComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.TESMagicTargetForm_Component:
                return MagicTargetFormComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSStoredTraversals_Component:
                return StoredTraversalsComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSAddToInventoryOnDestroy_Component:
                return AddToInventoryOnDestroyComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSCrowdComponent_Component:
                return CrowdComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSSpaceshipHullCode_Component:
                return SpaceshipHullCodeComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSPrimitive_Component:
                return PrimitiveComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSSpawnOnDestroy_Component:
                return SpawnOnDestroyComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.BGSWorldSpaceOverlay_Component:
                return WorldSpaceOverlayComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.ReflectionProbes_Component:
                return ReflectionProbesComponent.CreateFromBinary(frame, translationParams);
            case ComponentType.TESImageSpaceModifiableForm_Component:
                return ImageSpaceModifiableFormComponent.CreateFromBinary(frame, translationParams);
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
            IPlanetContentManagerContentPropertiesComponentGetter _ => AComponent.ComponentType.BGSPlanetContentManagerContentProperties_Component,
            IShipManagementComponentGetter _ => AComponent.ComponentType.BGSShipManagement,
            IExternalDataSourceComponentGetter _ => AComponent.ComponentType.BGSExternalComponentDataSource_Component,
            IForcedLocRefTypeComponentGetter _ => AComponent.ComponentType.BGSForcedLocRefType_Component,
            ILinkedVoiceTypeComponentGetter _ => AComponent.ComponentType.BGSLinkedVoiceType_Component,
            IPapyrusScriptsComponentGetter _ => AComponent.ComponentType.BGSPapyrusScripts_Component,
            IPathingDataComponentGetter _ => AComponent.ComponentType.BGSPathingData_Component,
            IPropertySheetComponentGetter _ => AComponent.ComponentType.BGSPropertySheet_Component,
            ISpaceshipAIActorComponentGetter _ => AComponent.ComponentType.BGSSpaceshipAIActor_Component,
            ISpaceshipEquipmentComponentGetter _ => AComponent.ComponentType.BGSSpaceshipEquipment_Component,
            ISpaceshipWeaponBindingsComponentGetter _ => AComponent.ComponentType.BGSSpaceshipWeaponBindings_Component,
            IFormLinkDataComponentGetter _ => AComponent.ComponentType.BGSFormLinkData_Component,
            IBlueprintComponentGetter _ => AComponent.ComponentType.Blueprint_Component,
            IDestructibleObjectComponentGetter _ => AComponent.ComponentType.BGSDestructibleObject_Component,
            IContainerComponentGetter _ => AComponent.ComponentType.TESContainer_Component,
            IMagicTargetFormComponentGetter _ => AComponent.ComponentType.TESMagicTargetForm_Component,
            IStoredTraversalsComponentGetter _ => AComponent.ComponentType.BGSStoredTraversals_Component,
            IAddToInventoryOnDestroyComponentGetter _ => AComponent.ComponentType.BGSAddToInventoryOnDestroy_Component,
            ICrowdComponentGetter _ => AComponent.ComponentType.BGSCrowdComponent_Component,
            ISpaceshipHullCodeComponentGetter _ => AComponent.ComponentType.BGSSpaceshipHullCode_Component,
            IPrimitiveComponentGetter _ => AComponent.ComponentType.BGSPrimitive_Component,
            ISpawnOnDestroyComponentGetter _ => AComponent.ComponentType.BGSSpawnOnDestroy_Component,
            IWorldSpaceOverlayComponentGetter _ => AComponent.ComponentType.BGSWorldSpaceOverlay_Component,
            IReflectionProbesComponentGetter _ => AComponent.ComponentType.ReflectionProbes_Component,
            IImageSpaceModifiableFormComponentGetter _ => AComponent.ComponentType.TESImageSpaceModifiableForm_Component,
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
                return PlanetContentManagerContentPropertiesComponentBinaryOverlay.PlanetContentManagerContentPropertiesComponentFactory(stream, package);
            case AComponent.ComponentType.BGSShipManagement:
                return ShipManagementComponentBinaryOverlay.ShipManagementComponentFactory(stream, package);
            case AComponent.ComponentType.BGSExternalComponentDataSource_Component:
                return ExternalDataSourceComponentBinaryOverlay.ExternalDataSourceComponentFactory(stream, package);
            case AComponent.ComponentType.BGSForcedLocRefType_Component:
                return ForcedLocRefTypeComponentBinaryOverlay.ForcedLocRefTypeComponentFactory(stream, package);
            case AComponent.ComponentType.BGSLinkedVoiceType_Component:
                return LinkedVoiceTypeComponentBinaryOverlay.LinkedVoiceTypeComponentFactory(stream, package);
            case AComponent.ComponentType.BGSPapyrusScripts_Component:
                return PapyrusScriptsComponentBinaryOverlay.PapyrusScriptsComponentFactory(stream, package);
            case AComponent.ComponentType.BGSPathingData_Component:
                return PathingDataComponentBinaryOverlay.PathingDataComponentFactory(stream, package);
            case AComponent.ComponentType.BGSPropertySheet_Component:
                return PropertySheetComponentBinaryOverlay.PropertySheetComponentFactory(stream, package);
            case AComponent.ComponentType.BGSSpaceshipAIActor_Component:
                return SpaceshipAIActorComponentBinaryOverlay.SpaceshipAIActorComponentFactory(stream, package);
            case AComponent.ComponentType.BGSSpaceshipEquipment_Component:
                return SpaceshipEquipmentComponentBinaryOverlay.SpaceshipEquipmentComponentFactory(stream, package);
            case AComponent.ComponentType.BGSSpaceshipWeaponBindings_Component:
                return SpaceshipWeaponBindingsComponentBinaryOverlay.SpaceshipWeaponBindingsComponentFactory(stream, package);
            case AComponent.ComponentType.BGSFormLinkData_Component:
                return FormLinkDataComponentBinaryOverlay.FormLinkDataComponentFactory(stream, package);
            case AComponent.ComponentType.Blueprint_Component:
                return BlueprintComponentBinaryOverlay.BlueprintComponentFactory(stream, package);
            case AComponent.ComponentType.BGSDestructibleObject_Component:
                return DestructibleObjectComponentBinaryOverlay.DestructibleObjectComponentFactory(stream, package);
            case AComponent.ComponentType.TESContainer_Component:
                return ContainerComponentBinaryOverlay.ContainerComponentFactory(stream, package);
            case AComponent.ComponentType.TESMagicTargetForm_Component:
                return MagicTargetFormComponentBinaryOverlay.MagicTargetFormComponentFactory(stream, package);
            case AComponent.ComponentType.BGSStoredTraversals_Component:
                return StoredTraversalsComponentBinaryOverlay.StoredTraversalsComponentFactory(stream, package);
            case AComponent.ComponentType.BGSAddToInventoryOnDestroy_Component:
                return AddToInventoryOnDestroyComponentBinaryOverlay.AddToInventoryOnDestroyComponentFactory(stream, package);
            case AComponent.ComponentType.BGSCrowdComponent_Component:
                return CrowdComponentBinaryOverlay.CrowdComponentFactory(stream, package);
            case AComponent.ComponentType.BGSSpaceshipHullCode_Component:
                return SpaceshipHullCodeComponentBinaryOverlay.SpaceshipHullCodeComponentFactory(stream, package);
            case AComponent.ComponentType.BGSPrimitive_Component:
                return PrimitiveComponentBinaryOverlay.PrimitiveComponentFactory(stream, package);
            case AComponent.ComponentType.BGSSpawnOnDestroy_Component:
                return SpawnOnDestroyComponentBinaryOverlay.SpawnOnDestroyComponentFactory(stream, package);
            case AComponent.ComponentType.BGSWorldSpaceOverlay_Component:
                return WorldSpaceOverlayComponentBinaryOverlay.WorldSpaceOverlayComponentFactory(stream, package);
            case AComponent.ComponentType.ReflectionProbes_Component:
                return ReflectionProbesComponentBinaryOverlay.ReflectionProbesComponentFactory(stream, package);
            case AComponent.ComponentType.TESImageSpaceModifiableForm_Component:
                return ImageSpaceModifiableFormComponentBinaryOverlay.ImageSpaceModifiableFormComponentFactory(stream, package);
            default:
                throw new NotImplementedException();
        }
        
    }
}