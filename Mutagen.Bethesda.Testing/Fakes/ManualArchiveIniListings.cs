using Mutagen.Bethesda.Archives.DI;
using Noggog;

namespace Mutagen.Bethesda.Testing.Fakes;

public class ManualArchiveIniListings : IGetArchiveIniListings
{
    private readonly List<FileName> _fileNames = new();
    
    public void SetTo(IEnumerable<FileName> fileNames)
    {
        _fileNames.SetTo(fileNames);
    }
    
    public void SetTo(params FileName[] fileNames)
    {
        _fileNames.SetTo(fileNames);
    }

    public IEnumerable<FileName>? TryGet()
    {
        return _fileNames.Count == 0 ? null : _fileNames;
    }
    
    public IEnumerable<FileName> Get()
    {
        return _fileNames;
    }
    
    public IEnumerable<FileName> Get(FilePath path)
    {
        throw new NotImplementedException();
    }
    
    public IEnumerable<FileName> Get(Stream iniStream)
    {
        throw new NotImplementedException();
    }
}