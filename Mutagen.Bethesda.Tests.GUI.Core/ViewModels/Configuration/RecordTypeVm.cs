using System.Windows.Input;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;

namespace Mutagen.Bethesda.Tests.GUI;

public class RecordTypeVm
{
    public RecordType RecordType { get; }
    public ICommand DeleteSkippedCommand { get; }
    public ICommand DeleteIncludedCommand { get; }

    public RecordTypeVm(MainVM mvm, RecordType recordType)
    {
        RecordType = recordType;
        DeleteSkippedCommand = ReactiveCommand.Create(() => mvm.SkippedRecordTypes.Remove(this));
        DeleteIncludedCommand = ReactiveCommand.Create(() => mvm.InterestingRecordTypes.Remove(this));
    }
}