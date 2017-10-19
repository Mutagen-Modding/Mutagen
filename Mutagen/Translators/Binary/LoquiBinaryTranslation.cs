using Loqui;
using Noggog;
using Noggog.Notifying;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Binary
{
    public class LoquiBinaryTranslation<T, M> : IBinaryTranslation<T, M>
        where T : ILoquiObjectGetter
        where M : class, IErrorMask, new()
    {
        public static readonly LoquiBinaryTranslation<T, M> Instance = new LoquiBinaryTranslation<T, M>();
        private static readonly ILoquiRegistration Registration = LoquiRegistration.GetRegister(typeof(T));
        public delegate T CREATE_FUNC(MutagenFrame reader, bool doMasks, out M errorMask);
        private static readonly Lazy<CREATE_FUNC> CREATE = new Lazy<CREATE_FUNC>(GetCreateFunc);
        public delegate void WRITE_FUNC(MutagenWriter writer, T item, bool doMasks, out M errorMask);
        private static readonly Lazy<WRITE_FUNC> WRITE = new Lazy<WRITE_FUNC>(GetWriteFunc);

        private IEnumerable<KeyValuePair<ushort, object>> EnumerateObjects(
            ILoquiRegistration registration,
            MutagenFrame reader,
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
            MutagenFrame frame,
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
                frame,
                skipProtected,
                doMasks,
                maskGet);
            var copyIn = LoquiRegistration.GetCopyInFunc<C>();
            copyIn(fields, item);
            mask = maskObj;
        }

        public static CREATE_FUNC GetCreateFunc()
        {
            var tType = typeof(T);
            var mType = typeof(M);
            var options = tType.GetMethods()
                .Where((methodInfo) => methodInfo.Name.Equals("Create_Binary"))
                .Where((methodInfo) => methodInfo.IsStatic
                    && methodInfo.IsPublic)
                .Where((methodInfo) => methodInfo.ReturnType.InheritsFrom(typeof(ValueTuple<,>)))
                .Where((methodInfo) => methodInfo.ReturnType.GenericTypeArguments[0].Equals(tType))
                .ToArray();
            var method = options
                .Where((methodInfo) => mType.InheritsFrom(methodInfo.ReturnType.GenericTypeArguments[1]))
                .Where((methodInfo) => methodInfo.ReturnType.Equals(typeof(ValueTuple<T, M>)))
                .FirstOrDefault();
            if (method != null)
            {
                var func = DelegateBuilder.BuildDelegate<Func<MutagenFrame, bool, (T item, M mask)>>(method);
                return (MutagenFrame reader, bool doMasks, out M errorMask) =>
                {
                    var ret = func(reader, doMasks);
                    errorMask = ret.mask;
                    return ret.item;
                };
            }
            method = options
                .Where((methodInfo) => typeof(M).InheritsFrom(methodInfo.ReturnType.GenericTypeArguments[1], couldInherit: true)).First();
            var f = DelegateBuilder.BuildGenericDelegate<Func<MutagenFrame, bool, (T item, M mask)>>(tType, new Type[] { mType.GenericTypeArguments[0] }, method);
            return (MutagenFrame reader, bool doMasks, out M errorMask) =>
            {
                var ret = f(reader, doMasks);
                errorMask = ret.mask;
                return ret.item;
            };
        }

        public static WRITE_FUNC GetWriteFunc()
        {
            var method = typeof(T).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where((methodInfo) => methodInfo.Name.Equals("Write_Binary_Internal"))
                .First();
            if (!method.IsGenericMethod)
            {
                var f = DelegateBuilder.BuildDelegate<Func<T, MutagenWriter, bool, object>>(method);
                return (MutagenWriter writer, T item, bool doMasks, out M errorMask) =>
                {
                    errorMask = (M)f(item, writer, doMasks);
                };
            }
            else
            {
                var f = DelegateBuilder.BuildGenericDelegate<Func<T, MutagenWriter, bool, object>>(typeof(T), new Type[] { typeof(M).GenericTypeArguments[0] }, method);
                return (MutagenWriter writer, T item, bool doMasks, out M errorMask) =>
                {
                    errorMask = (M)f(item, writer, doMasks);
                };
            }
        }

        public TryGet<T> Parse(MutagenFrame reader, bool doMasks, out MaskItem<Exception, M> mask)
        {
            try
            {
                var ret = TryGet<T>.Succeed(CREATE.Value(
                    reader: reader,
                    doMasks: doMasks,
                    errorMask: out var subMask));
                mask = subMask == null ? null : new MaskItem<Exception, M>(null, subMask);
                return ret;
            }
            catch (Exception ex)
            when (doMasks)
            {
                mask = new MaskItem<Exception, M>(ex, default(M));
                return TryGet<T>.Failure;
            }
        }

        public TryGet<T> Parse(MutagenFrame reader, bool doMasks, out M mask)
        {
            var ret = Parse(reader, doMasks, out MaskItem<Exception, M> subMask);
            if (subMask?.Overall != null)
            {
                throw subMask.Overall;
            }
            mask = subMask?.Specific;
            return ret;
        }

        void IBinaryTranslation<T, M>.Write(MutagenWriter writer, T item, ContentLength length, bool doMasks, out M mask)
        {
            throw new NotImplementedException();
        }

        public void Write(MutagenWriter writer, T item, bool doMasks, out MaskItem<Exception, M> mask)
        {
            try
            {
                WRITE.Value(
                    writer: writer,
                    item: item,
                    doMasks: doMasks,
                    errorMask: out var subMask);
                mask = subMask == null ? null : new MaskItem<Exception, M>(null, subMask);
            }
            catch (Exception ex)
            when (doMasks)
            {
                mask = new MaskItem<Exception, M>(ex, default(M));
            }
        }

        public TryGet<T> Parse(MutagenFrame reader, ContentLength length, bool doMasks, out M maskObj)
        {
            throw new NotImplementedException();
        }

        public TryGet<T> Parse(MutagenFrame reader, RecordType header, ContentLength lengthLength, bool doMasks, out M maskObj)
        {
            throw new NotImplementedException();
        }
    }
}
