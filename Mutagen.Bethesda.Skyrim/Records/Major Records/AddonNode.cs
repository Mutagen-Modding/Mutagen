using Mutagen.Bethesda.Binary;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class AddonNode
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IModelGetter? IModeledGetter.Model => this.Model;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IObjectBoundsGetter IObjectBoundedGetter.ObjectBounds => this.ObjectBounds;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ObjectBounds? IObjectBoundedOptional.ObjectBounds
        {
            get => this.ObjectBounds;
            set => this.ObjectBounds = value ?? new ObjectBounds();
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IObjectBoundsGetter? IObjectBoundedOptionalGetter.ObjectBounds => this.ObjectBounds;
        #endregion
    }

    namespace Internals
    {
        public partial class AddonNodeBinaryCreateTranslation
        {
            static partial void FillBinaryAlwaysLoadedCustom(MutagenFrame frame, IAddonNodeInternal item)
            {
                var flags = frame.ReadUInt16();
                item.AlwaysLoaded = flags switch 
                {
                    1 => false,
                    3 => true,
                    _ => throw new NotImplementedException()
                };
            }
        }

        public partial class AddonNodeBinaryWriteTranslation
        {
            static partial void WriteBinaryAlwaysLoadedCustom(MutagenWriter writer, IAddonNodeGetter item)
            {
                if (item.AlwaysLoaded)
                {
                    writer.Write((short)3);
                }
                else
                {
                    writer.Write((short)1);
                }
            }
        }

        public partial class AddonNodeBinaryOverlay
        {
            public Boolean GetAlwaysLoadedCustom() => BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(_AlwaysLoadedLocation)) switch
            {
                1 => false,
                3 => true,
                _ => throw new NotImplementedException()
            };
        }
    }
}
