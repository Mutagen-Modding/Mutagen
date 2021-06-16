using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.UnitTests
{
    public class LoadMetadataReferenceFixture
    {
        public List<MetadataReference> MetadataReferences { get; } = new List<MetadataReference>();

        public bool LoadMutagen = true;
        
        public LoadMetadataReferenceFixture()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.IsDynamic) continue;
                if (Want(assembly))
                {
                    MetadataReferences.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }
        }

        public bool Want(Assembly assembly)
        {
            var fileName = Path.GetFileName(assembly.Location);
            if (LoadMutagen
                && fileName.Contains("Mutagen")
                && fileName != "Mutagen.Bethesda.UnitTests.dll")
            {
                return true;
            }

            return false;
        }
    }
}