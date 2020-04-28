using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Race
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequired.Name
        {
            get => this.Name ?? string.Empty;
            set => this.Name = value;
        }

        internal static readonly RecordType NAM2 = new RecordType("NAM2");
        public bool ExportingExtraNam2 { get; set; }
        
        [Flags]
        public enum MajorFlag
        {
            Critter = 0x0008_0000
        }

        public enum Size
        {
            Small,
            Medium,
            Large,
            ExtraLarge
        }
    }

    public partial interface IRace
    {
        new bool ExportingExtraNam2 { get; set; }
    }

    public partial interface IRaceGetter
    {
        bool ExportingExtraNam2 { get; }
    }

    namespace Internals
    {
        public partial class RaceBinaryCreateTranslation
        {
            public const int NumBipedObjectNames = 32;

            static partial void FillBinaryExtraNAM2Custom(MutagenFrame frame, IRaceInternal item)
            {
                if (frame.Complete) return;
                if (frame.TryGetSubrecord(Race.NAM2, out var subHeader))
                {
                    item.ExportingExtraNam2 = true;
                    frame.Position += subHeader.TotalLength;
                }
            }

            static partial void FillBinaryBipedObjectNamesCustom(MutagenFrame frame, IRaceInternal item)
            {
                for (int i = 0; i < NumBipedObjectNames; i++)
                {
                    var subHeader = frame.Reader.ReadSubrecordFrame(Race_Registration.NAME_HEADER);
                    BipedObject type = (BipedObject)i;
                    var val = BinaryStringUtility.ProcessWholeToZString(subHeader.Content);
                    if (!string.IsNullOrEmpty(val))
                    {
                        item.BipedObjectNames[type] = val;
                    }
                }
            }

            static partial void FillBinaryFaceFxPhonemesListingParsingCustom(MutagenFrame frame, IRaceInternal item) => FaceFxPhonemesBinaryCreateTranslation.ParseFaceFxPhonemes(frame, item.FaceFxPhonemes);

            static partial void FillBinaryFaceFxPhonemesRawParsingCustom(MutagenFrame frame, IRaceInternal item) => FaceFxPhonemesBinaryCreateTranslation.ParseFaceFxPhonemes(frame, item.FaceFxPhonemes);
        }

        public partial class RaceBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;

            public bool ExportingExtraNam2 { get; private set; }
            public bool ExportingExtraNam3 => throw new NotImplementedException();

            private int? _faceFxPhonemesLoc;
            public IFaceFxPhonemesGetter FaceFxPhonemes => GetFaceFx();

            partial void ExtraNAM2CustomParse(BinaryMemoryReadStream stream, int offset)
            {
            }

            private int? _bipedObjectNamesLoc;
            public IReadOnlyDictionary<BipedObject, string> BipedObjectNames
            {
                get
                {
                    if (_bipedObjectNamesLoc == null) return DictionaryExt.Empty<BipedObject, string>();
                    var ret = new Dictionary<BipedObject, string>();
                    var loc = _bipedObjectNamesLoc.Value;
                    for (int i = 0; i < RaceBinaryCreateTranslation.NumBipedObjectNames; i++)
                    {
                        var subHeader = _package.Meta.SubrecordFrame(_data.Slice(loc).Span, Race_Registration.NAME_HEADER);
                        BipedObject type = (BipedObject)i;
                        var val = BinaryStringUtility.ProcessWholeToZString(subHeader.Content);
                        if (!string.IsNullOrEmpty(val))
                        {
                            ret[type] = val;
                        }
                        loc += subHeader.HeaderAndContentData.Length;
                    }
                    return ret;
                }
            }

            void BipedObjectNamesCustomParse(BinaryMemoryReadStream stream, int finalPos, int offset)
            {
                _bipedObjectNamesLoc = (ushort)(stream.Position - offset);
                UtilityTranslation.SkipPastAll(stream, _package.Meta, Race_Registration.NAME_HEADER);
            }

            partial void FaceFxPhonemesListingParsingCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                FaceFxPhonemesRawParsingCustomParse(stream, offset);
            }

            partial void FaceFxPhonemesRawParsingCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                if (_faceFxPhonemesLoc == null)
                {
                    _faceFxPhonemesLoc = (ushort)(stream.Position - offset);
                }
                UtilityTranslation.SkipPastAll(stream, _package.Meta, Race_Registration.PHTN_HEADER);
                UtilityTranslation.SkipPastAll(stream, _package.Meta, Race_Registration.PHWT_HEADER);
            }

            private FaceFxPhonemes GetFaceFx()
            {
                var ret = new FaceFxPhonemes();
                if (_faceFxPhonemesLoc == null) return ret;
                var frame = new MutagenFrame(new MutagenMemoryReadStream(_data.Slice(_faceFxPhonemesLoc.Value), _package.Meta));
                FaceFxPhonemesBinaryCreateTranslation.ParseFaceFxPhonemes(frame, ret);
                return ret;
            }
        }

        public partial class RaceBinaryWriteTranslation
        {
            static partial void WriteBinaryExtraNAM2Custom(MutagenWriter writer, IRaceGetter item)
            {
                if (item.ExportingExtraNam2)
                {
                    using var header = HeaderExport.ExportSubrecordHeader(writer, Race.NAM2);
                }
            }

            static partial void WriteBinaryBipedObjectNamesCustom(MutagenWriter writer, IRaceGetter item)
            {
                var bipedObjs = item.BipedObjectNames;
                for (int i = 0; i < RaceBinaryCreateTranslation.NumBipedObjectNames; i++)
                {
                    var bipedObj = (BipedObject)i;
                    using (HeaderExport.ExportSubrecordHeader(writer, Race_Registration.NAME_HEADER))
                    {
                        if (bipedObjs.TryGetValue(bipedObj, out var val))
                        {
                            writer.Write(val, StringBinaryType.NullTerminate);
                        }
                        else
                        {
                            writer.Write(string.Empty, StringBinaryType.NullTerminate);
                        }
                    }
                }
            }

            static partial void WriteBinaryFaceFxPhonemesListingParsingCustom(MutagenWriter writer, IRaceGetter item) => FaceFxPhonemesBinaryWriteTranslation.WriteFaceFxPhonemes(writer, item.FaceFxPhonemes);

            static partial void WriteBinaryFaceFxPhonemesRawParsingCustom(MutagenWriter writer, IRaceGetter item)
            {
                // Handled by Listing section
            }
        }
    }
}
