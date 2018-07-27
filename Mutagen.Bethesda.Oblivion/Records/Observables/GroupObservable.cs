using Loqui;
using Mutagen.Bethesda.Binary;
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
        where T : class, ILoquiObject<T>, IFormID
    {
        public static readonly RecordType T_RecordType;

        #region ContainedRecordType
        protected String _ContainedRecordType;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public String ContainedRecordType
        {
            get => this._ContainedRecordType;
            protected set => this._ContainedRecordType = value;
        }
        #endregion
        #region GroupType
        protected GroupTypeEnum _GroupType;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public GroupTypeEnum GroupType
        {
            get => this._GroupType;
            set => this._GroupType = value;
        }
        #endregion
        #region LastModified
        protected Byte[] _LastModified = new byte[4];
        public Byte[] LastModified
        {
            get => this._LastModified;
            set => this._LastModified = value;
        }
        #endregion
        #region Items
        private Dictionary<FormID, IObservable<T>> _observable { get; } = new Dictionary<FormID, IObservable<T>>();
        public IDictionaryGetter<FormID, IObservable<T>> Items => new DictionaryGetterWrapper<FormID, IObservable<T>>(_observable);
        public IObservable<T> ItemsObservable { get; }
        #endregion

        static GroupObservable()
        {
            T_RecordType = (RecordType)LoquiRegistration.GetRegister(typeof(T)).GetType().GetField(Mutagen.Bethesda.Constants.TRIGGERING_RECORDTYPE_MEMBER).GetValue(null);
        }

        public GroupObservable(long pos, Func<MutagenFrame> streamGetter)
        {
            using (var frame = streamGetter())
            {
                frame.Reader.Position = pos + 8;
                if (EnumBinaryTranslation<GroupTypeEnum>.Instance.Parse(
                    frame: frame.SpawnWithLength(4),
                    item: out GroupTypeEnum GroupTypeParse,
                    errorMask: null))
                {
                    this._GroupType = GroupTypeParse;
                }
                if (Mutagen.Bethesda.Binary.ByteArrayBinaryTranslation.Instance.Parse(
                    frame: frame.SpawnWithLength(4),
                    item: out Byte[] LastModifiedParse,
                    errorMask: null))
                {
                    this._LastModified = LastModifiedParse;
                }

                frame.Reader.Position = pos;
                foreach (var val in RecordLocator.ParseTopLevelGRUP(frame.Reader))
                {
                    this._observable[val.Key] = GetRecordObservable(val.Value, streamGetter);
                }
            }

            ItemsObservable = Observable.Create<T>(
                (o) =>
                {
                    try
                    {
                        var frame = streamGetter();
                        frame.Reader.Position = pos + 4;
                        var len = frame.Reader.ReadUInt32();
                        frame = frame.SpawnWithLength(len - 8);
                        using (frame)
                        {
                            frame.Reader.Position += 12;
                            while (!frame.Complete)
                            {
                                if (LoquiBinaryTranslation<T>.Instance.Parse(
                                    frame,
                                    out var item,
                                    errorMask: null))
                                {
                                    o.OnNext(item);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        o.OnError(ex);
                    }
                    o.OnCompleted();
                    return () => { };
                })
                .SubscribeOn(Scheduler.Default)
                .Publish()
                .RefCount();
        }

        private IObservable<T> GetRecordObservable(long pos, Func<MutagenFrame> streamGetter)
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
    }
}
