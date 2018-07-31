using Loqui;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public class GroupObservable<T>
        where T : MajorRecord, IFormID, ILoquiObject<T>
    {
        public static readonly RecordType T_RecordType;
        public static readonly GroupObservable<T> Empty = new GroupObservable<T>();

        public IObservable<GroupTypeEnum> GroupType { get; private set; }
        public IObservable<Byte[]> LastModified { get; private set; }

        public IObservable<KeyValuePair<FormID, IObservable<T>>> Items { get; private set; }

        static GroupObservable()
        {
            T_RecordType = (RecordType)LoquiRegistration.GetRegister(typeof(T)).GetType().GetField(Mutagen.Bethesda.Constants.TRIGGERING_RECORDTYPE_MEMBER).GetValue(null);
        }

        private GroupObservable()
        {
        }

        public static GroupObservable<T> FromStream(long pos, Func<MutagenFrame> streamGetter)
        {
            var ret = new GroupObservable<T>();
            using (var frame = streamGetter())
            {
                frame.Reader.Position = pos + 12;
                if (EnumBinaryTranslation<GroupTypeEnum>.Instance.Parse(
                    frame: frame.SpawnWithLength(4),
                    item: out GroupTypeEnum GroupTypeParse,
                    errorMask: null))
                {
                    ret.GroupType = Observable.Return(GroupTypeParse);
                }
                if (Mutagen.Bethesda.Binary.ByteArrayBinaryTranslation.Instance.Parse(
                    frame: frame.SpawnWithLength(4),
                    item: out Byte[] LastModifiedParse,
                    errorMask: null))
                {
                    ret.LastModified = Observable.Return(LastModifiedParse);
                }
            }

            ret.Items = Observable.Create<KeyValuePair<FormID, IObservable<T>>>(
                (o) =>
                {
                    try
                    {
                        using (var frame = streamGetter())
                        {
                            frame.Reader.Position = pos;
                            foreach (var val in RecordLocator.ParseTopLevelGRUP(frame.Reader))
                            {
                                o.OnNext(
                                    new KeyValuePair<FormID, IObservable<T>>(
                                        val.FormID,
                                        GetRecordStreamObservable(val.Position, streamGetter)));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        o.OnError(ex);
                    }
                    o.OnCompleted();
                    return ActionExt.Nothing;
                })
                .SubscribeOn(Scheduler.Default)
                .Publish()
                .RefCount()
                .Replay()
                .RefCount();

            return ret;
        }

        public GroupObservable<T> Where(Func<KeyValuePair<FormID, IObservable<MajorRecord>>, bool> selector)
        {
            return new GroupObservable<T>()
            {
                GroupType = this.GroupType,
                LastModified = this.LastModified,
                Items = this.Items.Where(
                    (o) => selector(
                        new KeyValuePair<FormID, IObservable<MajorRecord>>(
                            o.Key,
                            o.Value.Select<T, MajorRecord>((t) => t))))
            };
        }

        public GroupObservable<T> Do(Action<MajorRecord> doAction)
        {
            return new GroupObservable<T>()
            {
                GroupType = this.GroupType,
                LastModified = this.LastModified,
                Items = this.Items.SelectMany((i) => i.Value)
                    .Do(doAction)
                    .Cast<T>()
                    .Select((m) =>
                    {
                        return new KeyValuePair<FormID, IObservable<T>>(
                            m.FormID,
                            Observable.Return(m));
                    })
            };
        }

        public GroupObservable<T> With(
            IObservable<GroupTypeEnum> groupType)
        {
            return new GroupObservable<T>()
            {
                GroupType = groupType,
                LastModified = this.LastModified,
                Items = this.Items
            };
        }

        public GroupObservable<T> With(
            IObservable<byte[]> lastModified)
        {
            return new GroupObservable<T>()
            {
                GroupType = this.GroupType,
                LastModified = lastModified,
                Items = this.Items
            };
        }

        private static IObservable<T> GetRecordStreamObservable(long pos, Func<MutagenFrame> streamGetter)
        {
            return Observable.Create<T>(
                (o) =>
                {
                    using (var frame = streamGetter())
                    {
                        try
                        {
                            frame.Reader.Position = pos;
                            if (LoquiBinaryTranslation<T>.Instance.Parse(
                                frame,
                                out var item,
                                errorMask: null))
                            {
                                o.OnNext(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            o.OnError(ex);
                        }
                    }
                    o.OnCompleted();
                    return () => { };
                })
                .SubscribeOn(Scheduler.Default)
                .Publish()
                .RefCount();
        }

        public async Task Write(MutagenWriter writer)
        {
            using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
            {
                Mutagen.Bethesda.Binary.Int32BinaryTranslation.Instance.Write(
                    writer,
                    Group<T>.GRUP_RECORD_TYPE.TypeInt,
                    errorMask: null);
                Mutagen.Bethesda.Binary.EnumBinaryTranslation<GroupTypeEnum>.Instance.Write(
                    writer,
                    await this.GroupType.LastAsync(),
                    length: 4,
                    fieldIndex: (int)Group_FieldIndex.GroupType,
                    errorMask: null);
                Mutagen.Bethesda.Binary.ByteArrayBinaryTranslation.Instance.Write(
                    writer: writer,
                    item: await this.LastModified.LastAsync(),
                    fieldIndex: (int)Group_FieldIndex.LastModified,
                    errorMask: null);
                await this.Items.SelectMany((i) => i.Value)
                    .Do((i) =>
                    {
                        LoquiBinaryTranslation<T>.Instance.Write(
                            writer: writer,
                            item: i,
                            errorMask: null);
                    });
            }
        }
    }
}
