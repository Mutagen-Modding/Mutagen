using System.IO.Abstractions;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class DialogResponseEmotionNoneTests
{
    private static ModPath WriteResponseWithNoneEmotion(
        IFileSystem fileSystem,
        Fallout4Mod mod,
        DirectoryPath directory)
    {
        var quest = mod.Quests.AddNew();
        var topic = new DialogTopic(mod);
        quest.DialogTopics.Add(topic);
        var responses = new DialogResponses(mod);
        var response = new DialogResponse();
        response.Emotion.SetTo(FormKey.None);
        responses.Responses.Add(response);
        topic.Responses.Add(responses);

        var path = new ModPath(mod.ModKey, Path.Combine(directory, mod.ModKey.FileName));
        mod.BeginWrite
            .ToPath(path)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        return path;
    }

    private static FormKey EmotionOf(IFallout4ModGetter mod) =>
        mod.Quests.Single().DialogTopics.Single().Responses.Single().Responses.Single().Emotion.FormKey;

    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void MutableReadsEmotionOfNone(IFileSystem fileSystem, Fallout4Mod mod, DirectoryPath existingDirectory)
    {
        var path = WriteResponseWithNoneEmotion(fileSystem, mod, existingDirectory);

        var reimport = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(path)
            .Mutable()
            .WithFileSystem(fileSystem)
            .Construct();

        EmotionOf(reimport).ShouldBe(FormKey.None);
    }

    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void OverlayReadsEmotionOfNone(IFileSystem fileSystem, Fallout4Mod mod, DirectoryPath existingDirectory)
    {
        var path = WriteResponseWithNoneEmotion(fileSystem, mod, existingDirectory);

        using var reimport = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(path)
            .WithFileSystem(fileSystem)
            .Construct();

        EmotionOf(reimport).ShouldBe(FormKey.None);
    }
}
