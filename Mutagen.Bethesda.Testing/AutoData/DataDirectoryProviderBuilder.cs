using AutoFixture.Kernel;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Testing.Fakes;

namespace Mutagen.Bethesda.Testing.AutoData;

public class DataDirectoryProviderBuilder : ISpecimenBuilder
{
    private IDataDirectoryProvider? _dataDirectoryProvider;

    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type t)
        {
            if (t == typeof(IDataDirectoryProvider)
                || t == typeof(ManualDataDirectoryProvider))
            {
                if (_dataDirectoryProvider == null)
                {
                    _dataDirectoryProvider = new ManualDataDirectoryProvider();
                }
                return _dataDirectoryProvider;
            }
        }

        return new NoSpecimen();
    }
}