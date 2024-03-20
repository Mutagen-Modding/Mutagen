using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Binary.Headers;

namespace Mutagen.Bethesda.Fallout4;

partial class SceneAction
{
    public enum TypeEnum
    {
        Dialog = 0,
        Package = 1,
        Timer = 2,
        PlayerDialogue = 3,
        //StartScene,
        NpcResponseDialogue = 5,
        Radio = 6,
    }

    [Flags]
    public enum Flag : uint
    {
        PlayerPositiveUseDialogueSubtype = 0x0000_0080,
        PlayerNegativeUseDialogueSubtype = 0x0000_0100,
        PlayerNeutralUseDialogueSubtype = 0x0000_0200,
        UseDialogueSubtype = 0x0000_0400,
        PlayerQuestionUseDialogueSubtype = 0x0000_0800,
        ClearTargetOnActionEnd = 0x0000_1000,
        FaceTarget = 0x0000_8000,
        Looping = 0x0001_0000,
        HeadtrackPlayer = 0x0002_0000,
        IgnoreForCompletion = 0x0008_0000,
        CameraSpeakerTarget = 0x0020_0000,
        CompleteFaceTarget = 0x0040_0000,
        NpcPositiveUseDialogueSubtype = 0x0800_0000,
        NpcNegativeUseDialogueSubtype = 0x1000_0000,
        NpcNeutralUseDialogueSubtype = 0x2000_0000,
        NpcQuestionUseDialogueSubtype = 0x4000_0000,
    }
}

partial class SceneActionBinaryCreateTranslation
{
    public static partial void FillBinaryTypeCustom(
        MutagenFrame frame,
        ISceneAction item, 
        PreviousParse lastParsed)
    {
        var rec = frame.ReadSubrecord(RecordTypes.ANAM);
        var type = rec.AsUInt16();
        if (type == 4)
        {
            item.Type = new SceneActionStartScene();
        }
        else
        {
            item.Type = new SceneActionTypicalType()
            {
                Type = (SceneAction.TypeEnum)type
            };
        }
    }

    public static partial ParseResult FillBinaryHTIDParsingCustom(
        MutagenFrame frame,
        ISceneAction item,
        PreviousParse lastParsed)
    {
        var subRec = frame.ReadSubrecord(RecordTypes.HTID);
        switch (item.Type)
        {
            case SceneActionStartScene startScene:
                startScene.EndSceneSayGreeting = true;
                break;
            case SceneActionTypicalType typical:
                typical.PlaySound.SetTo(FormKeyBinaryTranslation.Instance.Parse(subRec.Content, frame.MetaData.MasterReferences));
                break;
            default:
                throw new NotImplementedException();
        }
        return lastParsed;
    }
}

partial class SceneActionBinaryWriteTranslation
{

    public static partial void WriteBinaryTypeCustom(
        MutagenWriter writer,
        ISceneActionGetter item)
    {
        using var header = HeaderExport.Subrecord(writer, RecordTypes.ANAM);
        switch (item.Type)
        {
            case ISceneActionStartSceneGetter startScene:
                writer.Write((ushort)4);
                break;
            case ISceneActionTypicalTypeGetter typical:
                writer.Write((ushort)typical.Type);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static partial void WriteBinaryHTIDParsingCustom(
        MutagenWriter writer,
        ISceneActionGetter item)
    {
        switch (item.Type)
        {
            case ISceneActionStartSceneGetter startScene:
                if (startScene.EndSceneSayGreeting != true) return;
                using (HeaderExport.Subrecord(writer, RecordTypes.HTID))
                {
                }
                break;
            case ISceneActionTypicalTypeGetter typical:
                if (typical.PlaySound.FormKeyNullable is { } fk)
                {
                    using (HeaderExport.Subrecord(writer, RecordTypes.HTID))
                    {
                        FormKeyBinaryTranslation.Instance.Write(writer, fk);
                    }
                }
                break;
            default:
                throw new NotImplementedException();
        }
    }
}

partial class SceneActionBinaryOverlay
{
    private IASceneActionTypeGetter _type = null!;
    public partial IASceneActionTypeGetter GetTypeCustom() => _type;

    partial void TypeCustomParse(
        OverlayStream stream,
        int finalPos,
        int offset)
    {
        var loc = (stream.Position - offset);
        var rec = _package.MetaData.Constants.Subrecord(_recordData.Slice(loc));
        var type = rec.AsUInt16();
        if (type == 4)
        {
            _type = new SceneActionStartScene();
        }
        else
        {
            _type = new SceneActionTypicalType()
            {
                Type = (SceneAction.TypeEnum)type
            };
        }
    }

    public partial ParseResult HTIDParsingCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        var subRec = stream.ReadSubrecord(RecordTypes.HTID);
        switch (this.Type)
        {
            case SceneActionStartScene startScene:
                startScene.EndSceneSayGreeting = true;
                break;
            case SceneActionTypicalType typical:
                typical.PlaySound.SetTo(FormKeyBinaryTranslation.Instance.Parse(subRec.Content, stream.MetaData.MasterReferences));
                break;
            default:
                throw new NotImplementedException();
        }
        return lastParsed;
    }
}
