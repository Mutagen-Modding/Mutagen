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
        
        private Dictionary<FormID, IObservable<T>> _observables = new Dictionary<FormID, IObservable<T>>();
        
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
        public Dictionary<FormID, IObservable<T>> Items => _observables;
        #endregion

        static GroupObservable()
        {
            T_RecordType = (RecordType)LoquiRegistration.GetRegister(typeof(T)).GetType().GetField(Mutagen.Bethesda.Constants.TRIGGERING_RECORDTYPE_MEMBER).GetValue(null);
        }

        public GroupObservable(Func<Stream> streamGetter, IEnumerable<KeyValuePair<FormID, long>> values)
        {
            foreach (var val in values)
            {
                _observables[val.Key] = Observable.Create<T>(
                    (o) =>
                    {
                        using (var frame = new MutagenFrame(
                            new BinaryReadStream(
                                streamGetter())))
                        {
                            try
                            {
                                frame.Reader.Position = val.Value;
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
            int wer = 23;
            wer++;
        }
    }
}
