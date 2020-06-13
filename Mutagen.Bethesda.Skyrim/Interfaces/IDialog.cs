using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records are allowed as a dialog target
    /// Implemented by: [DialogItem, DialogResponses]
    /// </summary>
    public interface IDialog : IMajorRecordCommon, IDialogGetter
    {
    }

    /// <summary>
    /// Used for specifying which records are allowed as a dialog target
    /// Implemented by: [DialogItem, DialogResponses]
    /// </summary>
    public interface IDialogGetter : IMajorRecordCommonGetter
    {
    }
}
