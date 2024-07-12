using System.Text;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Exceptions;

public class FormIDCompactionOutOfBoundsException : Exception
{
    public bool SmallMaster { get; }
    public bool MediumMaster { get; }
    public RangeUInt32 Range { get; }
    public FormLinkInformation OutOfBoundsRecord { get; }

    public FormIDCompactionOutOfBoundsException(bool small, bool Medium, RangeUInt32 range, IFormLinkIdentifier outOfBounds)
        : base("Record was out of bounds for the given record compaction settings")
    {
        SmallMaster = small;
        MediumMaster = Medium;
        Range = range;
        OutOfBoundsRecord = FormLinkInformation.Factory(outOfBounds);
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
            style = "Half Master";
        }
        StringBuilder sb = new();
        sb.Append(OutOfBoundsRecord);
        sb.AppendLine(" was outside the ");
        sb.AppendLine(style);
        sb.AppendLine(" range: [");
        sb.AppendLine(Range.Min.ToString());
        sb.AppendLine(",");
        sb.AppendLine(Range.Min.ToString());
        sb.AppendLine("]");
        return sb.ToString();
    }
}