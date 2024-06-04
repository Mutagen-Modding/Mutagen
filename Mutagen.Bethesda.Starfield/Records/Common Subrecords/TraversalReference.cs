using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Starfield.Internals;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

public partial class TraversalReference
{
    [Flags]
    public enum Flag
    {
        NoTraversalFormID = 0x4
    }
}

partial class TraversalReferenceBinaryCreateTranslation
{
    public const int HasNoTraversal = 0x4;

    public static readonly ITraversalReferenceGetter Zeroed = new TraversalReference()
    {
        // ToDo
        // Dont require Starfield modkey
        Traversal = FormKey.Factory("000000:Starfield.esm").ToNullableLink<ITraversalGetter>()
    };

    public static readonly TraversalReference.TranslationMask _equalsMask = new(false)
    {
        From = true,
        Unknown1 = true
    };
    
    public static partial void FillBinaryTraversalCustom(
        MutagenFrame frame,
        ITraversalReference item)
    {
        var flags = frame.ReadInt32();
        if (flags != HasNoTraversal)
        {
            item.Traversal.SetTo(FormKeyBinaryTranslation.Instance.Parse(frame));
        }
        else
        {
            item.Traversal.SetToNull();
        }

        item.Unknown = frame.ReadBytes(8);
    }
 
    public static ExtendedList<TraversalReference> Parse( 
        MutagenFrame reader,
        out uint fluffBytes)
    {
        fluffBytes = 0;
        var ret = new ExtendedList<TraversalReference>(); 
        while (!reader.Complete) 
        {
            var remaining = checked((uint)reader.Remaining);
            try
            {
                if (!TraversalReference.TryCreateFromBinary(reader, out var subItem)
                    || Zeroed.Equals(subItem, _equalsMask))
                {
                    fluffBytes = remaining;
                    reader.SkipRemainingBytes();
                    break;
                } 
            
                ret.Add(subItem); 
            }
            catch (ArgumentOutOfRangeException)
            {
                // Expected to occur within Starfield.esm, due to fluff bytes of unknown length
                fluffBytes = remaining;
                reader.SkipRemainingBytes();
                break;
            }
        } 
        return ret; 
    } 
}

partial class TraversalReferenceBinaryWriteTranslation
{
    public static partial void WriteBinaryTraversalCustom(
        MutagenWriter writer,
        ITraversalReferenceGetter item)
    {
        var trav = item.Traversal;
        if (trav.IsNull)
        {
            writer.Write(TraversalReferenceBinaryCreateTranslation.HasNoTraversal);
        }
        else
        {
            writer.Write(0);
            FormKeyBinaryTranslation.Instance.Write(writer, trav);
        }
        writer.Write(item.Unknown);
    }

    public static void TraversalsListWriterHelper(
        MutagenWriter writer,
        IReadOnlyList<ITraversalReferenceGetter>? items,
        uint numTraversalFluffBytes)
    {
        if (items == null) return; 
        try 
        { 
            try 
            { 
                using (HeaderExport.Subrecord( 
                           writer, 
                           RecordTypes.XTV2,  
                           overflowRecord: RecordTypes.XXXX, 
                           out var writerToUse)) 
                { 
                    foreach (var item in items) 
                    { 
                        ((TraversalReferenceBinaryWriteTranslation)((IBinaryItem)item).BinaryWriteTranslator).Write(
                            item: item,
                            writer: writerToUse);
                    } 
                    writerToUse.WriteZeros(numTraversalFluffBytes);
                } 
            } 
            catch (OverflowException overflow) 
            { 
                throw new OverflowException( 
                    $"Cell traversals had an overflow with {items?.Count} items.", 
                    overflow); 
            } 
        } 
        catch (Exception ex) 
        { 
            throw SubrecordException.Enrich(ex, RecordTypes.XTV2); 
        }
    }
}

partial class TraversalReferenceBinaryOverlay
{
    private bool HasFormKey => !Enums.HasFlag(BinaryPrimitives.ReadInt32LittleEndian(_structData.Slice(0x28)), 4);
    
    public partial IFormLinkNullableGetter<ITraversalGetter> GetTraversalCustom(int location)
    {
        if (HasFormKey)
        {
            return new FormLinkNullable<ITraversalGetter>(
                FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(
                    _structData.Slice(0x2C))));
        }
        else
        {
            return FormLinkNullable<ITraversalGetter>.Null;
        }
    }

    public ReadOnlyMemorySlice<byte> Unknown => _structData.Slice(HasFormKey ? 0x30 : 0x2C, 8);

    public static IReadOnlyList<ITraversalReferenceGetter> Factory(
        OverlayStream stream, BinaryOverlayFactoryPackage package,
        long finalPos, int offset, PreviousParse lastParsed,
        out uint fluffBytes)
    {
        stream.ReadSubrecordHeader(RecordTypes.XTV2);
        return BinaryOverlayList.FactoryByArray(
            mem: stream.RemainingMemory,
            package: package,
            getter: (s, p) =>
            {
                return TraversalReferenceFactory(s, p);
            },
            locs: GetLocs(stream, package, finalPos, offset, lastParsed, out fluffBytes));
    }

    public static IReadOnlyList<int> GetLocs(
        OverlayStream stream, BinaryOverlayFactoryPackage package,
        long finalPos, int offset, PreviousParse lastParsed,
        out uint fluffBytes)
    {
        List<int> locs = new();
        var startingPos = stream.Position;
        fluffBytes = 0;
        while (stream.Position < finalPos)
        {
            var remaining = checked((uint)(finalPos - stream.Position));
            try
            {
                var itemPos = stream.Position;
                var bytes = stream.ReadMemory(0x10);
                if (bytes.All(b => b == 0))
                {
                    fluffBytes = remaining;
                    break;
                }
                locs.Add(itemPos - startingPos);
                stream.Position += 0x18;
                var flags = stream.ReadInt32();
                if (Enums.HasFlag(flags, 4))
                {
                    stream.Position += 8;
                }
                else
                {
                    stream.Position += 12;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // Expected to occur within Starfield.esm, due to fluff bytes of unknown length
                fluffBytes = remaining;
                stream.Position += checked((int)remaining);
                break;
            }
        }

        return locs;
    }

    partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
    {
        stream.Position += 0x30 + (HasFormKey ? 8 : 4);
    }
}