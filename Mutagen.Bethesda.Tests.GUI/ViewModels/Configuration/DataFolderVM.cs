using Noggog.WPF;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Tests.GUI;

public class DataFolderVM : ViewModel
{
    public GameRelease GameRelease { get; }

    public PathPickerVM DataFolder { get; } = new PathPickerVM()
    {
        PathType = PathPickerVM.PathTypeOptions.Folder,
        ExistCheckOption = PathPickerVM.CheckOptions.IfPathNotEmpty
    };

    public DataFolderVM(GameRelease release)
    {
        GameRelease = release;
    }
}