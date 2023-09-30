using Mutagen.Bethesda.Fallout4;

namespace Loqui;

internal class ProtocolDefinition_Fallout4 : IProtocolRegistration
{
    public static readonly ProtocolKey ProtocolKey = new("Fallout4");
    void IProtocolRegistration.Register() => Register();
    public static void Register()
    {
        LoquiRegistration.Register(
            AAudioEffect_Registration.Instance,
            AColorRecordData_Registration.Instance,
            AcousticSpace_Registration.Instance,
            ActionRecord_Registration.Instance,
            ActivateParent_Registration.Instance,
            ActivateParents_Registration.Instance,
            Activator_Registration.Instance,
            ActorValueInformation_Registration.Instance,
            ADamageType_Registration.Instance,
            AddonNode_Registration.Instance,
            AHolotapeData_Registration.Instance,
            AimModel_Registration.Instance,
            ALocationTarget_Registration.Instance,
            Alpha_Registration.Instance,
            AlphaLayer_Registration.Instance,
            AMagicEffectArchetype_Registration.Instance,
            AmbientColors_Registration.Instance,
            Ammunition_Registration.Instance,
            ANavigationMapInfoParent_Registration.Instance,
            ANavmeshParent_Registration.Instance,
            AnimatedObject_Registration.Instance,
            AnimationChangeThresholds_Registration.Instance,
            AnimationSoundTag_Registration.Instance,
            AnimationSoundTagSet_Registration.Instance,
            ANpcLevel_Registration.Instance,
            AObjectModification_Registration.Instance,
            AObjectModProperty_Registration.Instance,
            APackageData_Registration.Instance,
            APackageTarget_Registration.Instance,
            APerkEffect_Registration.Instance,
            APerkEntryPointEffect_Registration.Instance,
            APlacedTrap_Registration.Instance,
            AQuestAlias_Registration.Instance,
            Armor_Registration.Instance,
            ArmorAddon_Registration.Instance,
            ArmorAddonModel_Registration.Instance,
            ArmorModel_Registration.Instance,
            ArmorModification_Registration.Instance,
            ArmorResistance_Registration.Instance,
            ArtObject_Registration.Instance,
            ASceneActionType_Registration.Instance,
            ASoundDescriptor_Registration.Instance,
            AssociationType_Registration.Instance,
            AStoryManagerNode_Registration.Instance,
            ATopicReference_Registration.Instance,
            Attack_Registration.Instance,
            AttackData_Registration.Instance,
            AttractionRule_Registration.Instance,
            AudioCategoryMultiplier_Registration.Instance,
            AudioCategorySnapshot_Registration.Instance,
            AudioEffectChain_Registration.Instance,
            AVirtualMachineAdapter_Registration.Instance,
            BaseLayer_Registration.Instance,
            BendableSpline_Registration.Instance,
            BipedBodyTemplate_Registration.Instance,
            BipedObjectData_Registration.Instance,
            BodyData_Registration.Instance,
            BodyPart_Registration.Instance,
            BodyPartData_Registration.Instance,
            BodyTemplate_Registration.Instance,
            Bone_Registration.Instance,
            Book_Registration.Instance,
            BookActorValue_Registration.Instance,
            BookPerk_Registration.Instance,
            BookSpell_Registration.Instance,
            BookTeachesNothing_Registration.Instance,
            BookTeachTarget_Registration.Instance,
            Bounding_Registration.Instance,
            CameraPath_Registration.Instance,
            CameraShot_Registration.Instance,
            Cell_Registration.Instance,
            CellBlock_Registration.Instance,
            CellCombinedMeshReference_Registration.Instance,
            CellExteriorLod_Registration.Instance,
            CellGrid_Registration.Instance,
            CellLighting_Registration.Instance,
            CellMaxHeightData_Registration.Instance,
            CellNavmeshParent_Registration.Instance,
            CellSubBlock_Registration.Instance,
            CellWaterVelocity_Registration.Instance,
            Class_Registration.Instance,
            Climate_Registration.Instance,
            CloudLayer_Registration.Instance,
            CollectionAlias_Registration.Instance,
            CollisionLayer_Registration.Instance,
            ColorData_Registration.Instance,
            ColorFrame_Registration.Instance,
            ColorRecord_Registration.Instance,
            ColorRemappingIndex_Registration.Instance,
            CombatStyle_Registration.Instance,
            Component_Registration.Instance,
            Condition_Registration.Instance,
            ConditionData_Registration.Instance,
            ConditionFloat_Registration.Instance,
            ConditionGlobal_Registration.Instance,
            ConstructibleCreatedObjectCount_Registration.Instance,
            ConstructibleObject_Registration.Instance,
            ConstructibleObjectComponent_Registration.Instance,
            Container_Registration.Instance,
            ContainerEntry_Registration.Instance,
            ContainerItem_Registration.Instance,
            CreateReferenceToObject_Registration.Instance,
            CrimeValues_Registration.Instance,
            DamageType_Registration.Instance,
            DamageTypeIndexed_Registration.Instance,
            DamageTypeItem_Registration.Instance,
            Debris_Registration.Instance,
            DebrisModel_Registration.Instance,
            Decal_Registration.Instance,
            DefaultObject_Registration.Instance,
            DefaultObjectManager_Registration.Instance,
            DefaultObjectUse_Registration.Instance,
            DelayAudioEffect_Registration.Instance,
            DestructableData_Registration.Instance,
            Destructible_Registration.Instance,
            DestructionStage_Registration.Instance,
            DialogBranch_Registration.Instance,
            DialogResponse_Registration.Instance,
            DialogResponseFlags_Registration.Instance,
            DialogResponses_Registration.Instance,
            DialogResponsesAdapter_Registration.Instance,
            DialogSetParentQuestStage_Registration.Instance,
            DialogTopic_Registration.Instance,
            DialogView_Registration.Instance,
            DistantLod_Registration.Instance,
            Door_Registration.Instance,
            DoorTriangle_Registration.Instance,
            DualCastData_Registration.Instance,
            DynamicAttentuationValues_Registration.Instance,
            EdgeLink_Registration.Instance,
            Effect_Registration.Instance,
            EffectData_Registration.Instance,
            EffectShader_Registration.Instance,
            EnableParent_Registration.Instance,
            EncounterZone_Registration.Instance,
            EquipmentSlot_Registration.Instance,
            EquipType_Registration.Instance,
            Explosion_Registration.Instance,
            ExternalAliasLocation_Registration.Instance,
            ExternalAliasReference_Registration.Instance,
            ExtraData_Registration.Instance,
            FaceFxPhonemes_Registration.Instance,
            FaceMorph_Registration.Instance,
            Faction_Registration.Instance,
            FactionOwner_Registration.Instance,
            Fallout4Group_Registration.Instance,
            Fallout4ListGroup_Registration.Instance,
            Fallout4MajorRecord_Registration.Instance,
            Fallout4Mod_Registration.Instance,
            Fallout4ModHeader_Registration.Instance,
            FilterKeywordChance_Registration.Instance,
            FindMatchingRefFromEvent_Registration.Instance,
            FindMatchingRefNearAlias_Registration.Instance,
            Flora_Registration.Instance,
            Footstep_Registration.Instance,
            FootstepSet_Registration.Instance,
            FormList_Registration.Instance,
            FunctionConditionData_Registration.Instance,
            Furniture_Registration.Instance,
            FurnitureMarkerEntryPoints_Registration.Instance,
            FurnitureMarkerParameters_Registration.Instance,
            GameSetting_Registration.Instance,
            GameSettingBool_Registration.Instance,
            GameSettingFloat_Registration.Instance,
            GameSettingInt_Registration.Instance,
            GameSettingString_Registration.Instance,
            GameSettingUInt_Registration.Instance,
            GetEventData_Registration.Instance,
            Global_Registration.Instance,
            GlobalBool_Registration.Instance,
            GlobalFloat_Registration.Instance,
            GlobalInt_Registration.Instance,
            GlobalShort_Registration.Instance,
            GodRays_Registration.Instance,
            Grass_Registration.Instance,
            Hazard_Registration.Instance,
            HeadData_Registration.Instance,
            HeadPart_Registration.Instance,
            HeadPartReference_Registration.Instance,
            Holotape_Registration.Instance,
            HolotapeProgram_Registration.Instance,
            HolotapeSound_Registration.Instance,
            HolotapeTerminal_Registration.Instance,
            HolotapeVoice_Registration.Instance,
            Icons_Registration.Instance,
            IdleAnimation_Registration.Instance,
            IdleMarker_Registration.Instance,
            ImageSpace_Registration.Instance,
            ImageSpaceAdapter_Registration.Instance,
            Impact_Registration.Instance,
            ImpactData_Registration.Instance,
            ImpactDataSet_Registration.Instance,
            Ingestible_Registration.Instance,
            Ingredient_Registration.Instance,
            InstanceNamingRule_Registration.Instance,
            InstanceNamingRuleProperties_Registration.Instance,
            InstanceNamingRules_Registration.Instance,
            InstanceNamingRuleSet_Registration.Instance,
            IslandData_Registration.Instance,
            Key_Registration.Instance,
            KeyFrame_Registration.Instance,
            Keyword_Registration.Instance,
            Landscape_Registration.Instance,
            LandscapeMPCD_Registration.Instance,
            LandscapeTexture_Registration.Instance,
            LandscapeVertexHeightMap_Registration.Instance,
            Layer_Registration.Instance,
            LayerHeader_Registration.Instance,
            LensFlare_Registration.Instance,
            LensFlareSprite_Registration.Instance,
            LensFlareSpriteData_Registration.Instance,
            LeveledItem_Registration.Instance,
            LeveledItemEntry_Registration.Instance,
            LeveledItemEntryData_Registration.Instance,
            LeveledNpc_Registration.Instance,
            LeveledNpcEntry_Registration.Instance,
            LeveledNpcEntryData_Registration.Instance,
            LeveledSpell_Registration.Instance,
            Light_Registration.Instance,
            LightingTemplate_Registration.Instance,
            LinkedAlias_Registration.Instance,
            LinkedDoor_Registration.Instance,
            LinkedReferences_Registration.Instance,
            LoadScreen_Registration.Instance,
            LoadScreenRotation_Registration.Instance,
            LoadScreenZoom_Registration.Instance,
            Location_Registration.Instance,
            LocationAliasReference_Registration.Instance,
            LocationCell_Registration.Instance,
            LocationCellEnablePoint_Registration.Instance,
            LocationCellStaticReference_Registration.Instance,
            LocationCellUnique_Registration.Instance,
            LocationCoordinate_Registration.Instance,
            LocationFallback_Registration.Instance,
            LocationKeyword_Registration.Instance,
            LocationObjectId_Registration.Instance,
            LocationObjectType_Registration.Instance,
            LocationReference_Registration.Instance,
            LocationReferenceType_Registration.Instance,
            LocationTarget_Registration.Instance,
            LocationTargetRadius_Registration.Instance,
            LockData_Registration.Instance,
            MagicEffect_Registration.Instance,
            MagicEffectArchetype_Registration.Instance,
            MagicEffectBoundArchetype_Registration.Instance,
            MagicEffectCloakArchetype_Registration.Instance,
            MagicEffectEnhanceWeaponArchetype_Registration.Instance,
            MagicEffectGuideArchetype_Registration.Instance,
            MagicEffectLightArchetype_Registration.Instance,
            MagicEffectPeakValueModArchetype_Registration.Instance,
            MagicEffectSound_Registration.Instance,
            MagicEffectSpawnHazardArchetype_Registration.Instance,
            MagicEffectSummonCreatureArchetype_Registration.Instance,
            MappingSound_Registration.Instance,
            MaterialObject_Registration.Instance,
            MaterialSubstitution_Registration.Instance,
            MaterialSwap_Registration.Instance,
            MaterialType_Registration.Instance,
            Message_Registration.Instance,
            MessageButton_Registration.Instance,
            MiscItem_Registration.Instance,
            MiscItemComponent_Registration.Instance,
            Model_Registration.Instance,
            ModStats_Registration.Instance,
            MorphGroup_Registration.Instance,
            MorphPreset_Registration.Instance,
            MorphValue_Registration.Instance,
            MovableStatic_Registration.Instance,
            MovementData_Registration.Instance,
            MovementDataOverride_Registration.Instance,
            MovementDirectionData_Registration.Instance,
            MovementRotationData_Registration.Instance,
            MovementType_Registration.Instance,
            MusicTrack_Registration.Instance,
            MusicTrackLoopData_Registration.Instance,
            MusicType_Registration.Instance,
            MusicTypeData_Registration.Instance,
            NavigationDoorLink_Registration.Instance,
            NavigationMapInfo_Registration.Instance,
            NavigationMapInfoCellParent_Registration.Instance,
            NavigationMapInfoWorldspaceParent_Registration.Instance,
            NavigationMesh_Registration.Instance,
            NavigationMeshInfoMap_Registration.Instance,
            NavigationMeshObstacleManager_Registration.Instance,
            NavigationMeshObstacleManagerSubObject_Registration.Instance,
            NavmeshGeometry_Registration.Instance,
            NavmeshGridArray_Registration.Instance,
            NavmeshNode_Registration.Instance,
            NavmeshSet_Registration.Instance,
            NavmeshTriangle_Registration.Instance,
            NavmeshWaypoint_Registration.Instance,
            NeckFatAdjustmentsScale_Registration.Instance,
            NoOwner_Registration.Instance,
            Npc_Registration.Instance,
            NpcBodyMorphRegionValues_Registration.Instance,
            NpcFaceMorph_Registration.Instance,
            NpcFaceTintingLayer_Registration.Instance,
            NpcLevel_Registration.Instance,
            NpcModification_Registration.Instance,
            NpcMorph_Registration.Instance,
            NpcOwner_Registration.Instance,
            NpcSound_Registration.Instance,
            NpcWeight_Registration.Instance,
            ObjectBounds_Registration.Instance,
            ObjectBoundsFloat_Registration.Instance,
            ObjectEffect_Registration.Instance,
            ObjectModBoolProperty_Registration.Instance,
            ObjectModEnumProperty_Registration.Instance,
            ObjectModFloatProperty_Registration.Instance,
            ObjectModFormLinkFloatProperty_Registration.Instance,
            ObjectModFormLinkIntProperty_Registration.Instance,
            ObjectModification_Registration.Instance,
            ObjectModInclude_Registration.Instance,
            ObjectModIntProperty_Registration.Instance,
            ObjectModItem_Registration.Instance,
            ObjectModStringProperty_Registration.Instance,
            ObjectProperty_Registration.Instance,
            ObjectTemplate_Registration.Instance,
            ObjectTemplateInclude_Registration.Instance,
            ObjectVisibilityManager_Registration.Instance,
            ObjectVisibilityManagerItem_Registration.Instance,
            Outfit_Registration.Instance,
            OverdriveAudioEffect_Registration.Instance,
            Ownership_Registration.Instance,
            OwnerTarget_Registration.Instance,
            Package_Registration.Instance,
            PackageAdapter_Registration.Instance,
            PackageBranch_Registration.Instance,
            PackageDataBool_Registration.Instance,
            PackageDataFloat_Registration.Instance,
            PackageDataInt_Registration.Instance,
            PackageDataLocation_Registration.Instance,
            PackageDataObjectList_Registration.Instance,
            PackageDataTarget_Registration.Instance,
            PackageDataTopic_Registration.Instance,
            PackageEvent_Registration.Instance,
            PackageFlagsOverride_Registration.Instance,
            PackageIdles_Registration.Instance,
            PackageRoot_Registration.Instance,
            PackageScriptFragments_Registration.Instance,
            PackageTargetAlias_Registration.Instance,
            PackageTargetInterruptData_Registration.Instance,
            PackageTargetKeyword_Registration.Instance,
            PackageTargetObjectID_Registration.Instance,
            PackageTargetObjectType_Registration.Instance,
            PackageTargetReference_Registration.Instance,
            PackageTargetSelf_Registration.Instance,
            PackageTargetSpecificReference_Registration.Instance,
            PackageTargetUnknown_Registration.Instance,
            PackIn_Registration.Instance,
            Part_Registration.Instance,
            Patrol_Registration.Instance,
            PcLevelMult_Registration.Instance,
            Perk_Registration.Instance,
            PerkAbilityEffect_Registration.Instance,
            PerkAdapter_Registration.Instance,
            PerkCondition_Registration.Instance,
            PerkEntryPointAbsoluteValue_Registration.Instance,
            PerkEntryPointAddActivateChoice_Registration.Instance,
            PerkEntryPointAddLeveledItem_Registration.Instance,
            PerkEntryPointAddRangeToValue_Registration.Instance,
            PerkEntryPointModifyActorValue_Registration.Instance,
            PerkEntryPointModifyValue_Registration.Instance,
            PerkEntryPointModifyValues_Registration.Instance,
            PerkEntryPointSelectSpell_Registration.Instance,
            PerkEntryPointSelectText_Registration.Instance,
            PerkEntryPointSetText_Registration.Instance,
            PerkPlacement_Registration.Instance,
            PerkQuestEffect_Registration.Instance,
            PerkScriptFragment_Registration.Instance,
            PerkScriptFragments_Registration.Instance,
            Phoneme_Registration.Instance,
            PlacedArrow_Registration.Instance,
            PlacedBarrier_Registration.Instance,
            PlacedBeam_Registration.Instance,
            PlacedCone_Registration.Instance,
            PlacedFlame_Registration.Instance,
            PlacedHazard_Registration.Instance,
            PlacedMissile_Registration.Instance,
            PlacedNpc_Registration.Instance,
            PlacedObject_Registration.Instance,
            PlacedObjectLighting_Registration.Instance,
            PlacedObjectMapMarker_Registration.Instance,
            PlacedObjectRadio_Registration.Instance,
            PlacedObjectSpline_Registration.Instance,
            PlacedPrimitive_Registration.Instance,
            PlacedTrap_Registration.Instance,
            Portal_Registration.Instance,
            PowerGridConnection_Registration.Instance,
            PreCutMapEntry_Registration.Instance,
            PreferredPathing_Registration.Instance,
            ProjectedDecal_Registration.Instance,
            Projectile_Registration.Instance,
            Quest_Registration.Instance,
            QuestAdapter_Registration.Instance,
            QuestCollectionAlias_Registration.Instance,
            QuestData_Registration.Instance,
            QuestFragmentAlias_Registration.Instance,
            QuestLocationAlias_Registration.Instance,
            QuestLogEntry_Registration.Instance,
            QuestObjective_Registration.Instance,
            QuestObjectiveTarget_Registration.Instance,
            QuestReferenceAlias_Registration.Instance,
            QuestScriptFragment_Registration.Instance,
            QuestStage_Registration.Instance,
            Race_Registration.Instance,
            RaceWeight_Registration.Instance,
            RadioReceiver_Registration.Instance,
            Rank_Registration.Instance,
            RankPlacement_Registration.Instance,
            ReferenceAliasLocation_Registration.Instance,
            ReferenceGroup_Registration.Instance,
            Region_Registration.Instance,
            RegionArea_Registration.Instance,
            RegionData_Registration.Instance,
            RegionGrass_Registration.Instance,
            RegionGrasses_Registration.Instance,
            RegionLand_Registration.Instance,
            RegionMap_Registration.Instance,
            RegionObject_Registration.Instance,
            RegionObjects_Registration.Instance,
            RegionSound_Registration.Instance,
            RegionSounds_Registration.Instance,
            RegionWeather_Registration.Instance,
            Relation_Registration.Instance,
            Relationship_Registration.Instance,
            ResistanceDestructible_Registration.Instance,
            ReverbParameters_Registration.Instance,
            Scene_Registration.Instance,
            SceneAction_Registration.Instance,
            SceneActionStartScene_Registration.Instance,
            SceneActionTypicalType_Registration.Instance,
            SceneActor_Registration.Instance,
            SceneAdapter_Registration.Instance,
            SceneCamera_Registration.Instance,
            SceneCollection_Registration.Instance,
            SceneCollectionItem_Registration.Instance,
            ScenePhase_Registration.Instance,
            ScenePhaseFragment_Registration.Instance,
            ScenePhaseUnusedData_Registration.Instance,
            SceneScriptFragments_Registration.Instance,
            SceneSetParentQuestStage_Registration.Instance,
            ScriptBoolListProperty_Registration.Instance,
            ScriptBoolProperty_Registration.Instance,
            ScriptEntry_Registration.Instance,
            ScriptEntryStructs_Registration.Instance,
            ScriptFloatListProperty_Registration.Instance,
            ScriptFloatProperty_Registration.Instance,
            ScriptFragment_Registration.Instance,
            ScriptFragmentIndexed_Registration.Instance,
            ScriptFragments_Registration.Instance,
            ScriptFragmentsIndexed_Registration.Instance,
            ScriptIntListProperty_Registration.Instance,
            ScriptIntProperty_Registration.Instance,
            ScriptObjectListProperty_Registration.Instance,
            ScriptObjectProperty_Registration.Instance,
            ScriptProperty_Registration.Instance,
            ScriptStringListProperty_Registration.Instance,
            ScriptStringProperty_Registration.Instance,
            ScriptStructListProperty_Registration.Instance,
            ScriptStructProperty_Registration.Instance,
            ScriptVariableListProperty_Registration.Instance,
            ScriptVariableProperty_Registration.Instance,
            SeasonalIngredientProduction_Registration.Instance,
            ShaderParticleGeometry_Registration.Instance,
            SimpleModel_Registration.Instance,
            SoundCategory_Registration.Instance,
            SoundDescriptor_Registration.Instance,
            SoundDescriptorAutoweaponData_Registration.Instance,
            SoundDescriptorCompoundData_Registration.Instance,
            SoundDescriptorStandardData_Registration.Instance,
            SoundKeywordMapping_Registration.Instance,
            SoundLoopAndRumble_Registration.Instance,
            SoundMarker_Registration.Instance,
            SoundOutputChannel_Registration.Instance,
            SoundOutputChannels_Registration.Instance,
            SoundOutputData_Registration.Instance,
            SoundOutputModel_Registration.Instance,
            SoundRateOfFire_Registration.Instance,
            SoundRepeat_Registration.Instance,
            Spell_Registration.Instance,
            SplineConnection_Registration.Instance,
            SplineLink_Registration.Instance,
            StartScene_Registration.Instance,
            StateVariableFilterAudioEffect_Registration.Instance,
            Static_Registration.Instance,
            StaticCollection_Registration.Instance,
            StaticPart_Registration.Instance,
            StaticPlacement_Registration.Instance,
            StoryManagerBranchNode_Registration.Instance,
            StoryManagerEventNode_Registration.Instance,
            StoryManagerQuest_Registration.Instance,
            StoryManagerQuestNode_Registration.Instance,
            Subgraph_Registration.Instance,
            TalkingActivator_Registration.Instance,
            TeleportDestination_Registration.Instance,
            TemplateActors_Registration.Instance,
            Terminal_Registration.Instance,
            TerminalBodyText_Registration.Instance,
            TerminalHolotapeEntry_Registration.Instance,
            TerminalMenuItem_Registration.Instance,
            TextureSet_Registration.Instance,
            TintGroup_Registration.Instance,
            TintTemplateColor_Registration.Instance,
            TintTemplateOption_Registration.Instance,
            TopicReference_Registration.Instance,
            TopicReferenceSubtype_Registration.Instance,
            Transform_Registration.Instance,
            TransientType_Registration.Instance,
            Tree_Registration.Instance,
            UnknownObjectModification_Registration.Instance,
            VendorValues_Registration.Instance,
            VirtualMachineAdapter_Registration.Instance,
            VirtualMachineAdapterIndexed_Registration.Instance,
            VisualEffect_Registration.Instance,
            VoiceType_Registration.Instance,
            Water_Registration.Instance,
            WaterNoiseProperties_Registration.Instance,
            WaterReflection_Registration.Instance,
            WaterVelocity_Registration.Instance,
            Weapon_Registration.Instance,
            WeaponDamageType_Registration.Instance,
            WeaponExtraData_Registration.Instance,
            WeaponModification_Registration.Instance,
            Weather_Registration.Instance,
            WeatherAlpha_Registration.Instance,
            WeatherAmbientColorSet_Registration.Instance,
            WeatherColor_Registration.Instance,
            WeatherGodRays_Registration.Instance,
            WeatherMagic_Registration.Instance,
            WeatherSound_Registration.Instance,
            WeatherType_Registration.Instance,
            WorldDefaultLevelData_Registration.Instance,
            Worldspace_Registration.Instance,
            WorldspaceBlock_Registration.Instance,
            WorldspaceGridReference_Registration.Instance,
            WorldspaceLandDefaults_Registration.Instance,
            WorldspaceMap_Registration.Instance,
            WorldspaceMaxHeight_Registration.Instance,
            WorldspaceNavmeshParent_Registration.Instance,
            WorldspaceParent_Registration.Instance,
            WorldspaceReference_Registration.Instance,
            WorldspaceSubBlock_Registration.Instance,
            Zoom_Registration.Instance
        );
    }
}
