using Mutagen.Bethesda.Papyrus.Syntax;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;

namespace Mutagen.Bethesda.Papyrus.Semantic;

/// <summary>
/// A set of parsed scripts that can see each other: cross-script member resolution,
/// base chains, and imports only work within a codebase. Script names are looked
/// up case-insensitively; if the same script name appears twice, the last one wins.
/// Large codebases build their models and collect diagnostics in parallel; the
/// per-script work is independent and the shared caches are thread-safe.
/// </summary>
public sealed class PapyrusCodebase
{
    private readonly Dictionary<string, PapyrusSemanticModel> _models = new(StringComparer.OrdinalIgnoreCase);

    // Below this size, parallel scheduling overhead outweighs the win.
    private const int ParallelThreshold = 8;

    public PapyrusCodebase(IEnumerable<SyntaxTree> trees)
        : this(BuildModels(trees)) {}

    /// <summary>
    /// Creates a codebase from already-built models. Callers decide which models
    /// are reused (e.g. a project fork reusing unchanged documents); this constructor
    /// only wires up name lookup and base chains.
    /// </summary>
    public PapyrusCodebase(IReadOnlyList<PapyrusSemanticModel> models)
    {
        Models = models;

        foreach (var model in models)
        {
            _models[model.Script.Name] = model;
        }

        // Wire the codebase into each model and resolve base script references
        // now that every script is known. Base wiring is only reassigned when it
        // actually changed, so reused models keep their existing wiring when valid.
        foreach (var model in models)
        {
            model.Codebase = this;
            if (model.Script.BaseScriptName is not {} baseName) continue;

            var baseScript = GetModel(baseName)?.Script;
            if (ReferenceEquals(model.Script.BaseScript, baseScript)) continue;

            model.Script.BaseScript = baseScript;
        }
    }

    public IReadOnlyList<PapyrusSemanticModel> Models { get; }

    public PapyrusSemanticModel? GetModel(string scriptName) => _models.GetValueOrDefault(scriptName);

    private static PapyrusSemanticModel[] BuildModels(IEnumerable<SyntaxTree> trees)
    {
        var treeList = trees as IReadOnlyList<SyntaxTree> ?? [.. trees];
        var models = new PapyrusSemanticModel[treeList.Count];
        if (treeList.Count >= ParallelThreshold)
        {
            Parallel.For(0, models.Length, i => models[i] = new PapyrusSemanticModel(treeList[i], buildIndexes: true));
        }
        else
        {
            for (var i = 0; i < models.Length; i++)
            {
                models[i] = new PapyrusSemanticModel(treeList[i], buildIndexes: true);
            }
        }

        return models;
    }

    /// <summary>All diagnostics of every script in the codebase.</summary>
    public IReadOnlyList<PapyrusDiagnostic> GetDiagnostics()
    {
        if (Models.Count >= ParallelThreshold)
        {
            return Models.AsParallel().AsOrdered().SelectMany(model => model.GetDiagnostics()).ToArray();
        }
        return Models.SelectMany(model => model.GetDiagnostics()).ToArray();
    }
}
