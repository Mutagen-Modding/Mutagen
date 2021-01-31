using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Noggog;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda.Fallout4
{
    //public partial class GlobalBool
    //{
    //    public const char TRIGGER_CHAR = 'b';
    //    public override char TypeChar => TRIGGER_CHAR;

    //    public override float? RawFloat
    //    {
    //        get => this.Data.TryGet(out var data) ? (float)(data ? 1 : 0) : (float)(default ? 1 : 0);
    //        set
    //        {
    //            if (value.HasValue)
    //            {
    //                this.Data = value.Value != 0;
    //            }
    //            else
    //            {
    //                this.Data = default;
    //            }
    //        }
    //    }

    //    internal static GlobalBool Factory()
    //    {
    //        return new GlobalBool();
    //    }
    //}

    //namespace Internals
    //{
    //    //    public partial class GlobalBoolBinaryWriteTranslation
    //    //    {
    //    //        static void WriteBinaryDataCustom(MutagenWriter writer, IGlobalBoolGetter item)
    //    //        {
    //    //            if (!item.Data.TryGet(out var data)) return;
    //    //            using (HeaderExport.Subrecord(writer, RecordTypes.FLTV))
    //    //            {
    //    //                writer.Write((float)(data?1:0));
    //    //            }
    //    //        }
    //    //    }

    //    public partial class GlobalBoolBinaryOverlay
    //    {
    //        public override char TypeChar => GlobalBool.TRIGGER_CHAR;
    //        public override float? RawFloat => (float)((this.Data ?? default) ? 1 : 0);

    //        //private int? _DataLocation;
    //        //public bool GetDataIsSetCustom() => _DataLocation.HasValue;
    //        //public short GetDataCustom()
    //        //{
    //        //    return (short)HeaderTranslation.ExtractSubrecordMemory(_data, _DataLocation!.Value, _package.MetaData.Constants).Float();
    //        //}
    //        //partial void DataCustomParse(OverlayStream stream, long finalPos, int offset)
    //        //{
    //        //    _DataLocation = (ushort)(stream.Position - offset);
    //        //}
    //    }
    //}
}
