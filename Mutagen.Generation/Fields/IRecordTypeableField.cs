using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Generation
{
    public interface IRecordTypeableField
    {
        RecordType? RecordType { get; }
        bool Optional { get; }
        ulong? Length { get; }
    }

    public interface IRecordTypeableFieldSetter : IRecordTypeableField
    {
        new RecordType? RecordType { get; set; }
        new bool Optional { get; set; }
        new ulong? Length { get; set; }
    }

    public static class IRecordTypeableFieldExt
    {
        public static void LoadIRecordTypable(this IRecordTypeableFieldSetter typable, XElement node)
        {
            var recordAttr = node.GetAttribute("recordType");
            if (recordAttr != null)
            {
                typable.RecordType = new RecordType(recordAttr);
            }
            typable.Optional = node.GetAttribute<bool>("optional", false);
            if (typable.Optional && !typable.RecordType.HasValue)
            {
                throw new ArgumentException("Cannot have an optional field if it is not a record typed field.");
            }
            typable.Length = node.GetAttribute<ulong?>("length", null);
            if (typable.Length.HasValue && typable.RecordType.HasValue)
            {
                throw new ArgumentException("Cannot define both length and record type.");
            }
            if (!typable.Length.HasValue && !typable.RecordType.HasValue)
            {
                throw new ArgumentException("Have to define either length or record type.");
            }
        }
    }
}
