using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class UtilityTranslation
    {
        public delegate void RecordStructFill<R>(
            R record,
            MutagenFrame frame,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask);

        public delegate TryGet<int?> RecordTypeFill<R>(
            R record,
            MutagenFrame frame,
            RecordType nextRecordType,
            int contentLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter);

        public delegate TryGet<int?> RecordTypelessStructFill<R>(
            R record,
            MutagenFrame frame,
            int? lastParsed,
            RecordType nextRecordType,
            int contentLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter);

        public delegate TryGet<int?> ModRecordTypeFill<R, G>(
            R record,
            MutagenFrame frame,
            RecordType nextRecordType,
            int contentLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            G importMask,
            RecordTypeConverter recordTypeConverter);

        public static M MajorRecordParse<M>(
            M record,
            MutagenFrame frame,
            ErrorMaskBuilder errorMask,
            RecordType recType,
            RecordTypeConverter recordTypeConverter,
            MasterReferences masterReferences,
            RecordStructFill<M> fillStructs,
            RecordTypeFill<M> fillTyped)
            where M : MajorRecord
        {
            frame = frame.SpawnWithFinalPosition(HeaderTranslation.ParseRecord(
                frame.Reader,
                recType));
            fillStructs(
                record: record,
                frame: frame,
                masterReferences: masterReferences,
                errorMask: errorMask);
            if (fillTyped == null) return record;
            MutagenFrame targetFrame = frame;
            if (record.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed))
            {
                targetFrame = frame.Decompress();
            }
            while (!targetFrame.Complete)
            {
                var nextRecordType = HeaderTranslation.GetNextSubRecordType(
                    targetFrame.Reader,
                    contentLength: out var contentLength);
                var finalPos = targetFrame.Position + contentLength + Constants.SUBRECORD_LENGTH;
                var parsed = fillTyped(
                    record: record,
                    frame: targetFrame,
                    nextRecordType: nextRecordType,
                    contentLength: contentLength,
                    masterReferences: masterReferences,
                    errorMask: errorMask,
                    recordTypeConverter: recordTypeConverter);
                if (parsed.Failed) break;
                if (targetFrame.Position < finalPos)
                {
                    targetFrame.Position = finalPos;
                }
            }
            frame.SetToFinalPosition();
            return record;
        }

        public static M RecordParse<M>(
            M record,
            MutagenFrame frame,
            bool setFinal,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            if (setFinal)
            {
                frame.SetToFinalPosition();
            }
            return record;
        }
        
        public static M RecordParse<M>(
            M record,
            MutagenFrame frame,
            bool setFinal,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs,
            RecordTypeFill<M> fillTyped)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
                while (!frame.Complete)
                {
                    var nextRecordType = HeaderTranslation.GetNextSubRecordType(
                        reader: frame.Reader,
                        contentLength: out var contentLength);
                    var finalPos = frame.Position + contentLength + Constants.SUBRECORD_LENGTH;
                    var parsed = fillTyped(
                        record: record,
                        frame: frame,
                        nextRecordType: nextRecordType,
                        contentLength: contentLength,
                        masterReferences: masterReferences,
                        errorMask: errorMask,
                        recordTypeConverter: recordTypeConverter);
                    if (parsed.Failed) break;
                    if (frame.Position < finalPos)
                    {
                        frame.Position = finalPos;
                    }
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            if (setFinal)
            {
                frame.SetToFinalPosition();
            }
            return record;
        }

        public static M TypelessRecordParse<M>(
            M record,
            MutagenFrame frame,
            bool setFinal,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            if (setFinal)
            {
                frame.SetToFinalPosition();
            }
            return record;
        }

        public static M TypelessRecordParse<M>(
            M record,
            MutagenFrame frame,
            bool setFinal,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs,
            RecordTypelessStructFill<M> fillTyped)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
                int? lastParsed = null;
                while (!frame.Complete)
                {
                    var nextRecordType = HeaderTranslation.GetNextSubRecordType(
                        reader: frame.Reader,
                        contentLength: out var contentLength);
                    var finalPos = frame.Position + contentLength + Constants.SUBRECORD_LENGTH;
                    var parsed = fillTyped(
                        record: record,
                        frame: frame,
                        lastParsed: lastParsed,
                        nextRecordType: nextRecordType,
                        contentLength: contentLength,
                        masterReferences: masterReferences,
                        errorMask: errorMask,
                        recordTypeConverter: recordTypeConverter);
                    if (parsed.Failed) break;
                    if (frame.Position < finalPos)
                    {
                        frame.Position = finalPos;
                    }
                    lastParsed = parsed.Value;
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            if (setFinal)
            {
                frame.SetToFinalPosition();
            }
            return record;
        }

        public static G GroupParse<G>(
            G record,
            MutagenFrame frame,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<G> fillStructs,
            RecordTypeFill<G> fillTyped)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
                while (!frame.Complete)
                {
                    var nextRecordType = HeaderTranslation.GetNextSubRecordType(
                        reader: frame.Reader,
                        contentLength: out var contentLength);
                    var finalPos = frame.Position + contentLength + Constants.SUBRECORD_LENGTH;
                    var parsed = fillTyped(
                        record: record,
                        frame: frame,
                        nextRecordType: nextRecordType,
                        contentLength: contentLength,
                        masterReferences: masterReferences,
                        errorMask: errorMask,
                        recordTypeConverter: recordTypeConverter);
                    if (parsed.Failed) break;
                    if (frame.Position < finalPos)
                    {
                        frame.Position = finalPos;
                    }
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            frame.SetToFinalPosition();
            return record;
        }

        public static M ModParse<M, G>(
            M record,
            MutagenFrame frame,
            MasterReferences masterReferences,
            G importMask,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs,
            ModRecordTypeFill<M, G> fillTyped)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
                while (!frame.Complete)
                {
                    var nextRecordType = HeaderTranslation.GetNextType(
                        reader: frame.Reader,
                        finalPos: out var finalPos,
                        contentLength: out var contentLength);
                    var parsed = fillTyped(
                        record: record,
                        frame: frame,
                        importMask: importMask,
                        nextRecordType: nextRecordType,
                        contentLength: contentLength,
                        masterReferences: masterReferences,
                        errorMask: errorMask,
                        recordTypeConverter: recordTypeConverter);
                    if (parsed.Failed) break;
                    if (frame.Position < finalPos)
                    {
                        frame.Position = finalPos;
                    }
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            frame.SetToFinalPosition();
            return record;
        }
    }
}
