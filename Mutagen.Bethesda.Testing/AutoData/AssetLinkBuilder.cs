using System;
using System.IO.Abstractions;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
using Noggog.Testing.IO;

namespace Mutagen.Bethesda.Testing.AutoData;

public class AssetLinkBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        Type type;
        string? name;
        if (request is ParameterInfo p)
        {
            if (p.Name == null) return new NoSpecimen();
            type = p.ParameterType;
            name = p.Name;
        }
        else if (request is Type t)
        {
            type = t;
            name = null;
        }
        else
        {
            return new NoSpecimen();
        }
        
        if (!type.InheritsFrom(typeof(IAssetLinkGetter))) return new NoSpecimen(); 
        
        var link = (IAssetLink)Activator.CreateInstance(type);
        
        var existing = name != null && name.ContainsInsensitive("existing");
        var fileName =
            $"{name}{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}{link.Type.FileExtensions.First()}";
        
        link.RawPath = Path.Combine($"{PathingUtil.DrivePrefix}Data", link.Type.BaseFolder, fileName);
        if (existing)
        {
            var fs = context.Create<IFileSystem>();
            fs.Directory.CreateDirectory(Path.GetDirectoryName(link.RawPath));
            fs.File.WriteAllText(link.RawPath, string.Empty);
        }
        return link;
    }
}