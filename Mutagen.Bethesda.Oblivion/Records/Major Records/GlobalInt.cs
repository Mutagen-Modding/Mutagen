using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog.Notifying;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class GlobalInt
    {
        public const char TRIGGER_CHAR = 'l';

        public override float RawFloat
        {
            get => (float)this.Data;
            set
            {
                var val = (int)value;
                if (this.Data != val)
                {
                    this.Data = val;
                    this.RaisePropertyChanged();
                }
                else
                {
                    this.Data_IsSet = true;
                }
            }
        }

        internal static GlobalInt Factory()
        {
            return new GlobalInt();
        }

        partial void CustomCtor()
        {
            this.TypeChar = TRIGGER_CHAR;
            this.WhenAny(x => x.RawFloat)
                .DistinctUntilChanged()
                .Select(x => (int)Math.Round(x))
                .BindTo(this, x => x.Data);
            this.WhenAny(x => x.Data)
                .DistinctUntilChanged()
                .BindTo(this, x => x.RawFloat);
        }

        static partial void WriteBinary_Data_Custom(MutagenWriter writer, GlobalInt item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            using (HeaderExport.ExportSubRecordHeader(writer, GlobalInt_Registration.FLTV_HEADER))
            {
                writer.Write((float)item.Data);
            }
        }
    }
}
