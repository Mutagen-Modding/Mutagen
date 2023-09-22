using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

partial class MaterialSwap
{
    [Flags]
    public enum MajorFlag
    {
        CustomSwap = 0x0001_0000,
    }
}

partial class MaterialSwapBinaryCreateTranslation
{
    public const int NewFormVersion = 112;
    
    public static partial void FillBinaryFNAMParsingCustom(
        MutagenFrame frame,
        IMaterialSwapInternal item)
    {
        if (frame.MetaData.FormVersion >= NewFormVersion) return;
        var pos = frame.Position;
        frame.TryReadSubrecord(RecordTypes.EDID, out _);
        string? str = null;
        while (frame.SpawnAll().TryScanToRecord(
                   RecordTypes.FNAM, 
                   out var foundRecord, 
                   MaterialSubstitution_Registration.TriggerSpecs.AllRecordTypes))
        {
            var fnamStr = foundRecord.AsString(frame.MetaData.Encodings.NonTranslated);
            if (str != null && !str.Equals(fnamStr))
            {
                throw new MalformedDataException($"All FNAM strings should be the same");
            }
            
            str = fnamStr;
        }

        item.TreeFolder = str;
        frame.Position = pos;
    }

    public static partial void FillBinaryTreeFolderCustom(
        MutagenFrame frame,
        IMaterialSwapInternal item,
        PreviousParse lastParsed)
    {
        var header = frame.ReadSubrecordHeader();
        item.TreeFolder = StringBinaryTranslation.Instance.Parse(
            reader: frame.SpawnWithLength(header.ContentLength),
            stringBinaryType: StringBinaryType.NullTerminate);
    }

    public static partial void FillBinarySubstitutionsCustom(MutagenFrame frame, IMaterialSwapInternal item, PreviousParse lastParsed)
    {
        item.Substitutions.SetTo(
            Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<MaterialSubstitution>.Instance.Parse(
                reader: frame.SpawnAll(),
                triggeringRecord: MaterialSubstitution_Registration.TriggerSpecs,
                transl: MaterialSubstitution.TryCreateFromBinary));
    }
}

partial class MaterialSwapBinaryWriteTranslation
{
    public static partial void WriteBinaryFNAMParsingCustom(
        MutagenWriter writer,
        IMaterialSwapGetter item)
    {
    }

    public static partial void WriteBinaryTreeFolderCustom(
        MutagenWriter writer,
        IMaterialSwapGetter item)
    {
        if (writer.MetaData.FormVersion < MaterialSwapBinaryCreateTranslation.NewFormVersion)
        {
            return;
        }

        StringBinaryTranslation.Instance.WriteNullable(
            writer: writer,
            item: item.TreeFolder,
            header: RecordTypes.FNAM,
            binaryType: StringBinaryType.NullTerminate);
    }

    public static partial void WriteBinarySubstitutionsCustom(MutagenWriter writer, IMaterialSwapGetter item)
    {
        var treeFolder = item.TreeFolder;
        if (writer.MetaData.FormVersion >= MaterialSwapBinaryCreateTranslation.NewFormVersion || treeFolder == null)
        {
            Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IMaterialSubstitutionGetter>.Instance.Write(
                writer: writer,
                items: item.Substitutions,
                transl: (MutagenWriter subWriter, IMaterialSubstitutionGetter subItem, TypedWriteParams conv) =>
                {
                    var Item = subItem;
                    ((MaterialSubstitutionBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                        item: Item,
                        writer: subWriter,
                        translationParams: conv);
                });
        }
        else
        {
            Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IMaterialSubstitutionGetter>.Instance.Write(
                writer: writer,
                items: item.Substitutions,
                transl: (MutagenWriter subWriter, IMaterialSubstitutionGetter subItem, TypedWriteParams conv) =>
                {
                    var Item = subItem;
                    ((MaterialSubstitutionBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                        item: Item,
                        writer: subWriter,
                        translationParams: conv);
                    StringBinaryTranslation.Instance.Write(writer, treeFolder, RecordTypes.FNAM, StringBinaryType.NullTerminate);
                });
        }
    }
}

partial class MaterialSwapBinaryOverlay
{
    private int? _fnamLoc;
    private ushort? _formVersion;
    private int _offset;
    
    public partial String? GetTreeFolderCustom()
    {
        if (_formVersion >= MaterialSwapBinaryCreateTranslation.NewFormVersion)
        {
            return _fnamLoc.HasValue ? BinaryStringUtility.ProcessWholeToZString(HeaderTranslation.ExtractSubrecordMemory(_recordData, _fnamLoc.Value, _package.MetaData.Constants), encoding: _package.MetaData.Encodings.NonTranslated) : string.Empty;
        }
        else
        {
            string? str = null;
            foreach (var fnam in RecordSpanExtensions.FindAllOfSubrecord(
                         _recordData.Slice(_package.MetaData.Constants.MajorConstants.HeaderLength - _offset),
                         _package.MetaData.Constants, 
                         RecordTypes.FNAM))
            {
                
                var fnamStr = fnam.AsString(_package.MetaData.Encodings.NonTranslated);
                if (str != null && !str.Equals(fnamStr))
                {
                    throw new MalformedDataException($"All FNAM strings should be the same");
                }

                str = fnamStr;
            }

            return str;
        }
    }

    partial void TreeFolderCustomParse(
        OverlayStream stream,
        long finalPos,
        int offset)
    {
        _fnamLoc = (stream.Position - offset);
    }

    partial void CustomFactoryEnd(
        OverlayStream stream,
        int finalPos,
        int offset)
    {
        _formVersion = stream.MetaData.FormVersion;
        _offset = offset;
    }
}

partial class MaterialSubstitutionBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryFNAMParsingCustom(
        MutagenFrame frame, 
        IMaterialSubstitution item, 
        PreviousParse lastParsed)
    {
        // Handled in parent
        return (int)MaterialSubstitution_FieldIndex.ReplacementMaterial;
    }
}

partial class MaterialSubstitutionBinaryWriteTranslation
{
    public static partial void WriteBinaryFNAMParsingCustom(
        MutagenWriter writer, 
        IMaterialSubstitutionGetter item)
    {
        // Handled in parent
    }
}

partial class MaterialSubstitutionBinaryOverlay
{
    public partial ParseResult FNAMParsingCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        // Handled in parent
        return (int)MaterialSubstitution_FieldIndex.ReplacementMaterial;
    }
}