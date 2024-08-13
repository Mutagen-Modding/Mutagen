using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Starfield;

public partial class TerminalMenuItem
{
    [Flags]
    public enum Flag
    {
        TemplateItem = 0x01,
        StartsOpen = 0x02,
        OverrideItemText = 0x04,
        CreatesNewWindow = 0x08,
    }
}

partial class TerminalMenuItemBinaryCreateTranslation
{
    public enum TypeOption
    {
        DisplayText,
        SubmenuTerminalMenu,
        ActionReturnToDesktop,
        ActionReturnToPrevious,
        ActionForceRedraw,
        DataSlateBOOK,
    }
    
    public static partial void FillBinaryTypeParseCustom(
        MutagenFrame frame,
        ITerminalMenuItem item)
    {
        var type = (TypeOption)frame.ReadUInt16();
        switch (type)
        {
            case TypeOption.DisplayText:
                item.Target = new TerminalMenuItemDisplayText();
                break;
            case TypeOption.SubmenuTerminalMenu:
                item.Target = new TerminalMenuItemSubmenu();
                break;
            case TypeOption.ActionReturnToDesktop:
                item.Target = new TerminalMenuItemReturnToDesktop();
                break;
            case TypeOption.ActionReturnToPrevious:
                item.Target = new TerminalMenuItemReturnToPrevious();
                break;
            case TypeOption.ActionForceRedraw:
                item.Target = new TerminalMenuItemForceRedraw();
                break;
            case TypeOption.DataSlateBOOK:
                item.Target = new TerminalMenuItemDataslate();
                break;
            default:
                throw new ArgumentOutOfRangeException("Type", $"Type was unknown: {(ushort)type}");
        }

        frame.Position += 2;
    }

    public static partial ParseResult FillBinaryTargetParseCustom(
        MutagenFrame frame,
        ITerminalMenuItem item,
        PreviousParse lastParsed)
    {
        var subRec = frame.GetSubrecordHeader();
        switch (subRec.RecordTypeInt)
        {
            case RecordTypeInts.UNAM:
            case RecordTypeInts.TNAM:
            case RecordTypeInts.BNAM:
                item.Target.CopyInFromBinary(frame);
                break;
            default:
                throw new ArgumentException();
        }
        return (int)TerminalMenuItem_FieldIndex.Target;
    }
}

partial class TerminalMenuItemBinaryWriteTranslation
{
    public static partial void WriteBinaryTypeParseCustom(
        MutagenWriter writer,
        ITerminalMenuItemGetter item)
    {
        TerminalMenuItemBinaryCreateTranslation.TypeOption type = item.Target switch
        {
            ITerminalMenuItemDataslateGetter terminalMenuItemDataslateGetter 
                => TerminalMenuItemBinaryCreateTranslation.TypeOption.DataSlateBOOK,
            ITerminalMenuItemDisplayTextGetter terminalMenuItemDisplayTextGetter
                => TerminalMenuItemBinaryCreateTranslation.TypeOption.DisplayText,
            ITerminalMenuItemForceRedrawGetter terminalMenuItemForceRedrawGetter
                => TerminalMenuItemBinaryCreateTranslation.TypeOption.ActionForceRedraw,
            ITerminalMenuItemReturnToDesktopGetter terminalMenuItemReturnToDesktopGetter
                => TerminalMenuItemBinaryCreateTranslation.TypeOption.ActionReturnToDesktop,
            ITerminalMenuItemReturnToPreviousGetter terminalMenuItemReturnToPreviousGetter 
                => TerminalMenuItemBinaryCreateTranslation.TypeOption.ActionReturnToPrevious,
            ITerminalMenuItemSubmenuGetter terminalMenuItemSubmenuGetter 
                => TerminalMenuItemBinaryCreateTranslation.TypeOption.SubmenuTerminalMenu,
            _ => throw new ArgumentOutOfRangeException()
        };
        writer.Write((ushort)type);
        writer.WriteZeros(2);
    }

    public static partial void WriteBinaryTargetParseCustom(
        MutagenWriter writer,
        ITerminalMenuItemGetter item)
    {
        item.Target.WriteToBinary(writer);
    }
}

partial class TerminalMenuItemBinaryOverlay
{
    private int? _targetLocation;
    public IATerminalMenuItemTargetGetter Target => GetTarget();
    
    public partial ParseResult TargetParseCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        _targetLocation = (stream.Position - offset);
        return (int)TerminalMenuItem_FieldIndex.Target;
    }

    private IATerminalMenuItemTargetGetter GetTarget()
    {
        switch (Type)
        {
            case TerminalMenuItemBinaryCreateTranslation.TypeOption.DisplayText:
                return TerminalMenuItemDisplayTextBinaryOverlay.TerminalMenuItemDisplayTextFactory(_recordData.Slice(_targetLocation!.Value), _package);
            case TerminalMenuItemBinaryCreateTranslation.TypeOption.SubmenuTerminalMenu:
                return TerminalMenuItemSubmenuBinaryOverlay.TerminalMenuItemSubmenuFactory(_recordData.Slice(_targetLocation!.Value), _package);
            case TerminalMenuItemBinaryCreateTranslation.TypeOption.ActionReturnToDesktop:
                return new TerminalMenuItemReturnToDesktop();
            case TerminalMenuItemBinaryCreateTranslation.TypeOption.ActionReturnToPrevious:
                return new TerminalMenuItemReturnToPrevious();
            case TerminalMenuItemBinaryCreateTranslation.TypeOption.ActionForceRedraw:
                return new TerminalMenuItemForceRedraw();
            case TerminalMenuItemBinaryCreateTranslation.TypeOption.DataSlateBOOK:
                return TerminalMenuItemDataslateBinaryOverlay.TerminalMenuItemDataslateFactory(_recordData.Slice(_targetLocation!.Value), _package);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private TerminalMenuItemBinaryCreateTranslation.TypeOption Type
    {
        get
        {
            var i = _TypeParse_IsSet
                ? BinaryPrimitives.ReadUInt16LittleEndian(_recordData.Slice(_TypeParseLocation, 2))
                : default(UInt16);
            return (TerminalMenuItemBinaryCreateTranslation.TypeOption)i;
        }
    }
}