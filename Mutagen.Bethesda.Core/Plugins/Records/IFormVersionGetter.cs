namespace Mutagen.Bethesda.Plugins.Records;

public interface IFormVersionGetter
{
    ushort? FormVersion { get; }
}

public interface IFormVersionSetter : IFormVersionGetter
{
    new ushort FormVersion { get; set; }
}