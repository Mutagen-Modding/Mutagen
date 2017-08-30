using Loqui;
using Noggog;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Binary
{
    public class LoquiBinaryTranslation<T, M> : IBinaryTranslation<T, M>
        where T : ILoquiObjectGetter
        where M : IErrorMask, new()
    {
        public static readonly LoquiBinaryTranslation<T, M> Instance = new LoquiBinaryTranslation<T, M>();

        private IEnumerable<KeyValuePair<ushort, object>> EnumerateObjects(
            ILoquiRegistration registration,
            BinaryReader reader,
            bool skipProtected,
            bool doMasks,
            Func<IErrorMask> mask)
        {
            var ret = new List<KeyValuePair<ushort, object>>();
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                if (doMasks)
                {
                    mask().Overall = ex;
                }
                else
                {
                    throw;
                }
            }
            return ret;
        }

        public void CopyIn<C>(
            BinaryReader reader,
            C item,
            bool skipProtected,
            bool doMasks,
            out M mask,
            NotifyingFireParameters? cmds)
            where C : T, ILoquiObject
        {
            var maskObj = default(M);
            Func<IErrorMask> maskGet;
            if (doMasks)
            {
                maskGet = () =>
                {
                    if (maskObj == null)
                    {
                        maskObj = new M();
                    }
                    return maskObj;
                };
            }
            else
            {
                maskGet = null;
            }
            var fields = EnumerateObjects(
                item.Registration,
                reader,
                skipProtected,
                doMasks,
                maskGet);
            var copyIn = LoquiRegistration.GetCopyInFunc<C>();
            copyIn(fields, item);
            mask = maskObj;
        }

        public TryGet<T> Parse(BinaryReader reader, bool doMasks, out M mask)
        {
            var maskObj = default(M);
            Func<IErrorMask> maskGet;
            if (doMasks)
            {
                maskGet = () =>
                {
                    if (maskObj == null)
                    {
                        maskObj = new M();
                    }
                    return maskObj;
                };
            }
            else
            {
                maskGet = null;
            }
            try
            {
                var regis = LoquiRegistration.GetRegister(typeof(T));
                var fields = EnumerateObjects(
                    regis,
                    reader,
                    skipProtected: false,
                    doMasks: doMasks,
                    mask: maskGet);
                var create = LoquiRegistration.GetCreateFunc<T>();
                var ret = create(fields);
                mask = maskObj;
                return TryGet<T>.Succeed(ret);
            }
            catch (Exception ex)
            when (doMasks)
            {
                maskGet().Overall = ex;
                mask = maskObj;
                return TryGet<T>.Failure;
            }
        }

        public void Write(BinaryWriter writer, T item, bool doMasks, out M mask)
        {
            throw new NotImplementedException();
        }

        public TryGet<T> Parse(BinaryReader reader, long length, bool doMasks, out M maskObj)
        {
            throw new NotImplementedException();
        }

        public TryGet<T> Parse(BinaryReader reader, RecordType header, byte lengthLength, bool doMasks, out M maskObj)
        {
            throw new NotImplementedException();
        }
    }
}
