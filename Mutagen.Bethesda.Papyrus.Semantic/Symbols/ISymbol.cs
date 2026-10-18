namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

public interface ISymbol
{
    string Name { get; }
    SymbolKind Kind { get; }
    ISymbol? ContainingSymbol { get; }
    DeclaringSyntaxReference DeclaringSyntaxReference { get; }
}
