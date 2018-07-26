using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public class OblivionMod_Observable
    {
        public TES4 TES4 { get; private set; }
        public IObservable<GroupObservable<GameSetting>> GameSettings { get; private set; } = Observable.Empty<GroupObservable<GameSetting>>();

        public static OblivionMod_Observable Create_Binary(
            Func<Stream> streamSource)
        {
            var ret = new OblivionMod_Observable();
            var frame = new MutagenFrame(new BinaryReadStream(streamSource()));
            using (frame)
            {
                while (!frame.Complete)
                {
                    Fill_Binary_RecordTypes(
                        item: ret,
                        frame: frame,
                        streamSource: streamSource);
                }
            }
            return ret;
        }

        protected static void Fill_Binary_RecordTypes(
            OblivionMod_Observable item,
            MutagenFrame frame,
            Func<Stream> streamSource)
        {
            var nextRecordType = HeaderTranslation.GetNextType(
                reader: frame.Reader,
                contentLength: out var contentLength);
            switch (nextRecordType.TypeInt)
            {
                case 0x34534554: // TES4
                    item.TES4 = TES4.Create_Binary(
                        frame: frame,
                        errorMask: null,
                        recordTypeConverter: null);
                    break;
                default:
                    throw new ArgumentException("Mod did not start with TES4");
            }

            frame.Reader.Position = 0;
            foreach (var grup in RecordLocator.IterateBaseGroupLocations(frame.Reader))
            {
                switch (grup.Key.TypeInt)
                {
                    case 0x54534D47: // GMST
                        item.GameSettings = CreateGrupObservable<GameSetting>(grup.Value, streamSource);
                        break;
                    default:
                        continue;
                        //throw new ArgumentException($"Unknown GRUP type: {grup.Key.Type}");
                }
            }
        }

        protected static IObservable<GroupObservable<GameSetting>> CreateGrupObservable<T>(
            long pos,
            Func<Stream> streamSource)
            where T : class, ILoquiObject<T>, IFormID
        {
            return Observable.Create<GroupObservable<GameSetting>>(
                subscribe: (o) =>
                {
                    var stream = new BinaryReadStream(streamSource());
                    stream.Position = pos;
                    var ret = new GroupObservable<GameSetting>(streamSource,
                        RecordLocator.ParseTopLevelGRUP(stream));
                    o.OnNext(ret);
                    o.OnCompleted();
                    return ActionExt.Nothing;
                })
                .Publish()
                .RefCount();
        }

        private static bool IsSubLevelGRUP(
            IBinaryReadStream reader)
        {
            var targetRec = HeaderTranslation.ReadNextRecordType(reader);
            if (!targetRec.Equals(Group_Registration.GRUP_HEADER))
            {
                reader.Position -= 4;
                return false;
            }
            reader.Position += 8;
            var grupType = EnumBinaryTranslation<GroupTypeEnum>.Instance.ParseValue(MutagenFrame.ByLength(reader, 4));
            reader.Position -= 16;
            switch (grupType)
            {
                case GroupTypeEnum.InteriorCellBlock:
                case GroupTypeEnum.ExteriorCellBlock:
                case GroupTypeEnum.CellChildren:
                case GroupTypeEnum.WorldChildren:
                case GroupTypeEnum.TopicChildren:
                    return true;
                default:
                    return false;
            }
        }
    }
}
