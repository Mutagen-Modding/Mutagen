using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

partial class AudioEffectChainBinaryCreateTranslation
{
    public enum Types : uint
    {
        BsOverdrive = 0x864804BE,
        BsStateVariableFilter = 0xEF575F7F,
        BsDelayEffect = 0x18837B4F,
    }
    
    private static readonly Lazy<RecordTriggerSpecs> _recordSpecs = new Lazy<RecordTriggerSpecs>(() =>
    {
        var triggers = RecordCollection.Factory(RecordTypes.KNAM);
        var all = RecordCollection.Factory(
            RecordTypes.KNAM,
            RecordTypes.DNAM);
        return new RecordTriggerSpecs(allRecordTypes: all, triggeringRecordTypes: triggers);
    });
    
    public static partial void FillBinaryEffectsCustom(MutagenFrame frame, IAudioEffectChainInternal item, PreviousParse lastParsed)
    {
        item.Effects.SetTo( 
            Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<AAudioEffect>.Instance.Parse(
                reader: frame.SpawnAll(),
                triggeringRecord: _recordSpecs.Value,
                translationParams: null,
                transl: TryCreateFromBinary));
    }

    private static bool TryCreateFromBinary(
        MutagenFrame frame,
        out AAudioEffect item,
        TypedParseParams translationParams = default)
    {
        var knam = frame.ReadSubrecord();
        var type = (Types)knam.AsUInt32();
        frame.ReadSubrecordHeader(RecordTypes.DNAM);
        switch (type)
        {
            case Types.BsOverdrive:
                item = OverdriveAudioEffect.CreateFromBinary(frame);
                return true;
            case Types.BsStateVariableFilter:
                item = StateVariableFilterAudioEffect.CreateFromBinary(frame);
                break;
            case Types.BsDelayEffect:
                item = DelayAudioEffect.CreateFromBinary(frame);
                break;
            default:
                throw new MalformedDataException($"KNAM was unexpected type: {type}");
        }

        return true;
    }
}

partial class AudioEffectChainBinaryWriteTranslation
{
    public static partial void WriteBinaryEffectsCustom(MutagenWriter writer, IAudioEffectChainGetter item)
    {
        Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IAAudioEffectGetter>.Instance.Write(
            writer: writer,
            items: item.Effects,
            transl: (MutagenWriter subWriter, IAAudioEffectGetter subItem, TypedWriteParams conv) =>
            {
                using (HeaderExport.Subrecord(subWriter, RecordTypes.KNAM))
                {
                    var type = subItem switch
                    {
                        IOverdriveAudioEffectGetter => AudioEffectChainBinaryCreateTranslation.Types.BsOverdrive,
                        IStateVariableFilterAudioEffectGetter => AudioEffectChainBinaryCreateTranslation.Types.BsStateVariableFilter,
                        IDelayAudioEffectGetter => AudioEffectChainBinaryCreateTranslation.Types.BsDelayEffect,
                        _ => throw new NotImplementedException(),
                    };
                    subWriter.Write((uint)type);
                }

                using (HeaderExport.Subrecord(subWriter, RecordTypes.DNAM))
                {
                    subItem.WriteToBinary(subWriter);
                }
            });
    }
}

partial class AudioEffectChainBinaryOverlay
{
    public IReadOnlyList<IAAudioEffectGetter> Effects { get; private set; } = Array.Empty<IAAudioEffectGetter>();
    
    partial void EffectsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        this.Effects = this.ParseRepeatedTypelessSubrecord<IAAudioEffectGetter>(
            stream: stream,
            translationParams: null,
            trigger: RecordTypes.KNAM,
            factory: AudioEffectFactory);
    }

    public static IAAudioEffectGetter AudioEffectFactory(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        TypedParseParams parseParams = default)
    {
        var knam = stream.ReadSubrecord();
        var type = (AudioEffectChainBinaryCreateTranslation.Types)knam.AsUInt32();
        stream.ReadSubrecordHeader(RecordTypes.DNAM);
        switch (type)
        {
            case AudioEffectChainBinaryCreateTranslation.Types.BsOverdrive:
                return OverdriveAudioEffectBinaryOverlay.OverdriveAudioEffectFactory(stream, package);
            case AudioEffectChainBinaryCreateTranslation.Types.BsStateVariableFilter:
                return StateVariableFilterAudioEffectBinaryOverlay.StateVariableFilterAudioEffectFactory(stream, package);
            case AudioEffectChainBinaryCreateTranslation.Types.BsDelayEffect:
                return DelayAudioEffectBinaryOverlay.DelayAudioEffectFactory(stream, package);
                break;
            default:
                throw new MalformedDataException($"KNAM was unexpected type: {type}");
        }
    }
}

