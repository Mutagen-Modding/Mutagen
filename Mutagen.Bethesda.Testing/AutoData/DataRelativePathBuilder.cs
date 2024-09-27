using AutoFixture.Kernel;
using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Testing.AutoData;

public class DataRelativePathBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type t && t == typeof(DataRelativePath))
        {
            return new DataRelativePath(Path.GetRandomFileName());
        }
        
        return new NoSpecimen();
    }
}