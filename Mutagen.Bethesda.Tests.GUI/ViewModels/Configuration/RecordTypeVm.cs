using System.Windows.Input;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;

namespace Mutagen.Bethesda.Tests.GUI;

public class RecordTypeVm
{
    public RecordType RecordType { get; }
    public ICommand DeleteCommand { get; }

    public RecordTypeVm(MainVM mvm, RecordType recordType)
    {
        RecordType = recordType;
        DeleteCommand = ReactiveCommand.Create(() => mvm.SkippedRecordTypes.Remove(this));
    }
}