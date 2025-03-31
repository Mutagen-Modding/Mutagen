using System.Text;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Exceptions;

public class FormIDCompactionOutOfBoundsException : Exception
{
    public bool SmallMaster { get; }
    public bool MediumMaster { get; }
    public RangeUInt32 Range { get; }
    public FormLinkInformation? OutOfBoundsRecord { get; }

    public FormIDCompactionOutOfBoundsException(bool small, bool medium, RangeUInt32 range, IFormLinkIdentifier? outOfBounds = null)
        : base("Record was out of bounds for the given record compaction settings")
    {
        SmallMaster = small;
        MediumMaster = medium;
        Range = range;
        OutOfBoundsRecord = outOfBounds == null ? null : FormLinkInformation.Factory(outOfBounds);
    }

    public override string ToString()
    {
        string style = "Unknown";
        if (SmallMaster && MediumMaster)
        {
        }
        else if (SmallMaster)
        {
            style = "Small Master";
        }
        else
        {
            style = "Medium Master";
        }
        StringBuilder sb = new();
        sb.Append(OutOfBoundsRecord);
        sb.Append(" was outside the ");
        sb.Append(style);
        sb.Append(" range: [");
        sb.Append(Range.Min.ToString());
        sb.Append(",");
        sb.Append(Range.Min.ToString());
        sb.AppendLine("]");
        return sb.ToString();
    }
}