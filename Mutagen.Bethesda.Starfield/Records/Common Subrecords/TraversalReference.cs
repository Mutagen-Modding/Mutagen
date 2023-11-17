using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
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
        MutagenFrame reader)
    {
        var ret = new ExtendedList<TraversalReference>(); 
        while (!reader.Complete) 
        {
            try
            {
                if (!TraversalReference.TryCreateFromBinary(reader, out var subItem)
                    || Zeroed.Equals(subItem))
                {
                    reader.SkipRemainingBytes();
                    break;
                } 
            
                ret.Add(subItem); 
            }
            catch (ArgumentOutOfRangeException)
            {
                // Expected to occur within Starfield.esm, due to fluff bytes of unknown length
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
        if (item.Traversal is { } trav)
        {
            writer.Write(0);
            FormKeyBinaryTranslation.Instance.Write(writer, trav.FormKey);
        }
        else
        {
            writer.Write(TraversalReferenceBinaryCreateTranslation.HasNoTraversal);
        }
        writer.Write(item.Unknown);
    }
}

partial class TraversalReferenceBinaryOverlay
{
    public partial IFormLinkNullableGetter<ITraversalGetter> GetTraversalCustom(int location)
    {
        throw new NotImplementedException();
    }
    
    public ReadOnlyMemorySlice<byte> Unknown { get; }
}