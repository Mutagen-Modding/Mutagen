using Noggog.Reactive;
using Noggog.UI;

namespace Mutagen.Bethesda.Tests.GUI;

public class DataFolderVM : ViewModel
{
    public GameRelease GameRelease { get; }

    public PathPickerVM DataFolder { get; }

    public DataFolderVM(GameRelease release, ISchedulerProvider schedulerProvider, IPathPickerDialogProvider pathPickerDialogProvider)
    {
        GameRelease = release;
        DataFolder = new PathPickerVM(schedulerProvider, pathPickerDialogProvider)
        {
            PathType = PathPickerVM.PathTypeOptions.Folder,
            ExistCheckOption = PathPickerVM.CheckOptions.IfPathNotEmpty
        };
    }
}