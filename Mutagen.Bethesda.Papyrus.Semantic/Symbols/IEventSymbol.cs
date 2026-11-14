namespace Mutagen.Bethesda.Papyrus.Semantic.Symbols;

public interface IEventSymbol : ISymbol
{
	IReadOnlyList<IParameterSymbol> Parameters { get; }

	/// <summary>
	/// The local variables declared anywhere in the event body, in declaration order.
	/// The body's statements themselves belong to the bound-node layer.
	/// </summary>
	IReadOnlyList<IVariableSymbol> Locals { get; }
}
