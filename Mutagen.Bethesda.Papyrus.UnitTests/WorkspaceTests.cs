using Mutagen.Bethesda.Papyrus.Semantic;
using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.Papyrus.UnitTests;

/// <summary>
/// The workspace layer. PapyrusDocument wraps script text with its parsed
/// tree; PapyrusProject holds documents and builds compilations. Fork operations
/// (WithDocumentText/WithAddedDocument/WithRemovedDocument) produce new projects
/// whose compilations reuse the models of unchanged documents.
/// </summary>
public sealed class WorkspaceTests
{
    private const string BaseScript = """
        ScriptName BaseScript
        int BaseValue = 0
        Function BaseFunction()
        EndFunction
        """;

    private const string ChildScript = """
        ScriptName ChildScript extends BaseScript
        Function ChildFunction()
            BaseFunction()
        EndFunction
        """;

    private static PapyrusProject CreateProject() => PapyrusProject.FromDocuments(
    [
        PapyrusDocument.Parse(BaseScript),
        PapyrusDocument.Parse(ChildScript),
    ]);

    [Fact]
    public void Document_ParsesText_AndExposesScriptName()
    {
        var document = PapyrusDocument.Parse(BaseScript, filePath: @"C:\scripts\BaseScript.psc");

        document.Name.ShouldBe("BaseScript");
        document.Text.ShouldBe(BaseScript);
        document.FilePath.ShouldBe(@"C:\scripts\BaseScript.psc");
        document.SyntaxTree.Root.ShouldNotBeNull();
    }

    [Fact]
    public void Project_FromDocuments_ExposesDocumentsAndResolvesCrossScript()
    {
        var project = CreateProject();

        project.Documents.Count.ShouldBe(2);
        project.GetDocument("childscript")!.Name.ShouldBe("ChildScript"); // case-insensitive
        project.GetDocument("missing").ShouldBeNull();

        var model = project.Codebase.GetModel("ChildScript").ShouldNotBeNull();
        var body = model.GetBody("ChildFunction").ShouldNotBeNull();
        var call = body.DescendantNodes().OfType<CallExpression>().Single();
        call.Function.ShouldNotBeNull();
        call.Function.Name.ShouldBe("BaseFunction");
    }

    [Fact]
    public void Project_FromDirectories_LoadsScriptsFromDisk()
    {
        var dir = Path.Combine(Path.GetTempPath(), "papyrus-workspace-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            File.WriteAllText(Path.Combine(dir, "DiskScript.psc"), "ScriptName DiskScript\nint x = 0\n");
            File.WriteAllText(Path.Combine(dir, "Other.txt"), "not a script");

            var project = PapyrusProject.FromDirectories(dir);

            project.Documents.Count.ShouldBe(1);
            project.GetDocument("DiskScript").ShouldNotBeNull();
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public void Project_CompilationIsCached()
    {
        var project = CreateProject();

        project.Codebase.ShouldBeSameAs(project.Codebase);
    }

    [Fact]
    public void WithDocumentText_RebuildsOnlyTheChangedModel()
    {
        var project = CreateProject();
        var childModelBefore = project.Codebase.GetModel("ChildScript");

        var edited = project.UpdateDocument("ChildScript", ChildScript + "\nint Extra = 1\n");

        edited.ShouldNotBeSameAs(project);
        var baseModelAfter = edited.Codebase.GetModel("BaseScript").ShouldNotBeNull();
        var childModelAfter = edited.Codebase.GetModel("ChildScript").ShouldNotBeNull();

        // Unchanged document: model instance is reused.
        baseModelAfter.ShouldBeSameAs(project.Codebase.GetModel("BaseScript"));
        // Changed document: model is rebuilt.
        childModelAfter.ShouldNotBeSameAs(childModelBefore);
        childModelAfter.Script.Members.Count.ShouldBe(2); // ChildFunction + Extra
    }

    [Fact]
    public void WithDocumentText_RewiresBaseChain_WhenBaseIsEdited()
    {
        var project = CreateProject();
        var before = project.Codebase.GetModel("ChildScript")!.GetBody("ChildFunction")!
            .DescendantNodes().OfType<CallExpression>().Single();
        before.Function.ShouldNotBeNull();
        before.Function.Name.ShouldBe("BaseFunction");

        var editedBase = BaseScript.Replace(
            "Function BaseFunction()",
            "Function BaseFunction()\n    ; edited\nEndFunction\nFunction NewlyAdded()\nEndFunction\nFunction BaseFunction2()");
        var fork = project.UpdateDocument("BaseScript", editedBase);

        // The child's model is reused, but its base now exposes the new member.
        var childModel = fork.Codebase.GetModel("ChildScript").ShouldNotBeNull();
        childModel.ShouldBeSameAs(project.Codebase.GetModel("ChildScript"));
        var baseModel = fork.Codebase.GetModel("BaseScript").ShouldNotBeNull();
        baseModel.Script.Functions.Select(f => f.Name).ShouldContain("NewlyAdded");

        // A new call to the added function resolves through the rewired chain.
        var fork2 = fork.UpdateDocument("ChildScript", """
            ScriptName ChildScript extends BaseScript
            Function ChildFunction()
                NewlyAdded()
            EndFunction
            """);
        var call = fork2.Codebase.GetModel("ChildScript")!.GetBody("ChildFunction")!
            .DescendantNodes().OfType<CallExpression>().Single();
        call.Function.ShouldNotBeNull();
        call.Function.Name.ShouldBe("NewlyAdded");
    }

    [Fact]
    public void WithDocumentText_ScriptNameChange_UpdatesLookup()
    {
        var project = CreateProject();

        var fork = project.UpdateDocument("ChildScript", ChildScript.Replace("ChildScript", "RenamedScript"));

        fork.GetDocument("ChildScript").ShouldBeNull();
        fork.GetDocument("RenamedScript").ShouldNotBeNull();
        fork.Codebase.GetModel("RenamedScript").ShouldNotBeNull();
    }

    [Fact]
    public void WithDocumentText_UnknownScript_Throws()
    {
        var project = CreateProject();

        Should.Throw<ArgumentException>(() => project.UpdateDocument("Missing", "ScriptName Missing"));
    }

    [Fact]
    public void WithDocumentText_SurfaceDiagnosticsThroughReparse()
    {
        var project = CreateProject();
        project.Codebase.GetDiagnostics().Count.ShouldBe(0);

        var broken = project.UpdateDocument("ChildScript", "ScriptName ChildScript\nFunction Broken(\n");
        broken.Codebase.GetDiagnostics().Count.ShouldBeGreaterThan(0);

        var fixedProject = broken.UpdateDocument("ChildScript", ChildScript);
        fixedProject.Codebase.GetDiagnostics().Count.ShouldBe(0);
    }

    [Fact]
    public void WithAddedDocument_AndWithRemovedDocument_UpdateTheProject()
    {
        var project = CreateProject();

        var added = project.AddDocument(PapyrusDocument.Parse("ScriptName ThirdScript\nint y = 0\n"));
        added.GetDocument("ThirdScript").ShouldNotBeNull();
        added.Codebase.GetModel("ThirdScript").ShouldNotBeNull();
        project.GetDocument("ThirdScript").ShouldBeNull(); // original untouched

        var removed = added.RemoveDocument("thirdscript");
        removed.GetDocument("ThirdScript").ShouldBeNull();
        removed.Codebase.GetModel("ThirdScript").ShouldBeNull();
        removed.Documents.Count.ShouldBe(2);
    }

    [Fact]
    public void WithRemovedDocument_UnknownScript_Throws()
    {
        var project = CreateProject();

        Should.Throw<ArgumentException>(() => project.RemoveDocument("Missing"));
    }
}
