using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog.IO;

namespace Mutagen.Bethesda;

public static class IModExt
{

    /// <summary>
    /// Returns the top-level Group getter object associated with the given Major Record Type.
    /// </summary>
    /// <returns>Group getter object associated with the given Major Record Type</returns>
    /// <typeparam name="TMajor">The type of Major Record to get the Group for</typeparam>
    /// <exception cref="ArgumentException">
    /// An unexpected TMajor type will throw an exception.<br />
    /// Unexpected types include: <br />
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod) <br />
    ///   - Nested types, where there is not just one top level group that contains given type (Placed Objects) <br />
    ///   - A setter type is requested from a getter only object. <br />
    /// </exception>
    public static IGroupGetter<TMajor> GetTopLevelGroup<TMajor>(this IModGetter mod) 
        where TMajor : IMajorRecordGetter
    {
        return mod.TryGetTopLevelGroup<TMajor>() ?? throw new ArgumentException($"Unknown major record type: {typeof(TMajor)}");
    }

    /// <summary>
    /// Returns the top-level Group getter object associated with the given Major Record Type.
    /// </summary>
    /// <returns>Group getter object associated with the given Major Record Type</returns>
    /// <param name="type">The type of Major Record to get the Group for</param>
    /// <exception cref="ArgumentException">
    /// An unexpected TMajor type will throw an exception.<br />
    /// Unexpected types include: <br />
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod) <br />
    ///   - Nested types, where there is not just one top level group that contains given type (Placed Objects) <br />
    ///   - A setter type is requested from a getter only object. <br />
    /// </exception>
    public static IGroupGetter GetTopLevelGroup(this IModGetter mod, Type type)
    {
        return mod.TryGetTopLevelGroup(type) ?? throw new ArgumentException($"Unknown major record type: {type}");
    }

    /// <summary>
    /// Returns the Group object associated with the given Major Record Type.
    /// </summary>
    /// <returns>Group object associated with the given Major Record Type</returns>
    /// <typeparam name="TMajor">The type of Major Record to get the Group for</typeparam>
    /// <exception cref="ArgumentException">
    /// An unexpected TMajor type will throw an exception.
    /// Unexpected types include:
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)
    ///   - A setter type is requested from a getter only object.
    /// </exception>
    public static IGroup<TMajor> GetTopLevelGroup<TMajor>(this IMod mod)
        where TMajor : IMajorRecord
    {
        return mod.TryGetTopLevelGroup<TMajor>() ?? throw new ArgumentException($"Unknown major record type: {typeof(TMajor)}");
    }

    /// <summary>
    /// Returns the top-level Group getter object associated with the given Major Record Type.
    /// </summary>
    /// <returns>Group getter object associated with the given Major Record Type</returns>
    /// <param name="type">The type of Major Record to get the Group for</param>
    /// <exception cref="ArgumentException">
    /// An unexpected TMajor type will throw an exception.<br />
    /// Unexpected types include: <br />
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod) <br />
    ///   - Nested types, where there is not just one top level group that contains given type (Placed Objects) <br />
    ///   - A setter type is requested from a getter only object. <br />
    /// </exception>
    public static IGroup GetTopLevelGroup(this IMod mod, Type type)
    {
        return mod.TryGetTopLevelGroup(type) ?? throw new ArgumentException($"Unknown major record type: {type}");
    }

    public static MasterStyle GetMasterStyle(this IModFlagsGetter mod)
    {
        bool small = mod.CanBeSmallMaster && mod.IsSmallMaster;
        bool medium = mod.CanBeMediumMaster && mod.IsMediumMaster;
        
        if (small && medium)
        {
            throw new ModHeaderMalformedException(mod.ModKey, "Mod was both a light and medium master");
        }

        if (small) return MasterStyle.Small;
        if (medium) return MasterStyle.Medium;
        return MasterStyle.Full;
    }

    public static IMod Duplicate(this IModGetter mod, ModKey newModKey, BinaryWriteParameters? writeParameters = null, BinaryReadParameters? readParameters = null)
    {
        if (mod.ModKey.Type != newModKey.Type) throw new ArgumentException("ModKey types must match");

        var oldModKey = mod.ModKey;
        var fileSystem = new FileSystem();
        using var tmp = TempFolder.Factory();
        var fileSystemRoot = tmp.Dir;
        var oldModPath = new ModPath(oldModKey, fileSystem.Path.Combine(fileSystemRoot, oldModKey.FileName.String));

        // Write mod to in memory file system
        writeParameters ??= BinaryWriteParameters.Default;
        mod.WriteToBinary(oldModPath, writeParameters with
        {
            FileSystem = fileSystem
        });

        // Rename mod file
        var newModPath = new ModPath(newModKey, fileSystem.Path.Combine(fileSystemRoot, newModKey.FileName.String));
        fileSystem.File.Move(oldModPath, newModPath);

        // Rename strings files
        var stringsDir = fileSystem.Path.Combine(fileSystemRoot, "Strings");
        foreach (var file in fileSystem.Directory.EnumerateFiles(stringsDir))
        {
            var fileName = fileSystem.Path.GetFileName(file);
            if (fileName.StartsWith(oldModKey.Name))
            {
                fileSystem.File.Move(file, fileSystem.Path.Combine(stringsDir, newModKey.Name + fileName[oldModKey.Name.Length..]));
            }
        }

        // Read renamed mod as new mod
        readParameters ??= BinaryReadParameters.Default;
        var duplicateInto = ModInstantiator.ImportSetter(newModPath, mod.GameRelease, readParameters with
        {
            FileSystem = fileSystem
        });
        return duplicateInto;
    }
}