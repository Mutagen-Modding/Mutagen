using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using System.IO.Abstractions;
using System.Text;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary;

public class Utf8EncodingTests
{
    static Utf8EncodingTests()
    {
        // Register code pages encoding provider for CP1252 support in .NET Core
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <summary>
    /// Test string containing characters that differ between CP1252 and UTF8 encoding.
    /// Using Cyrillic characters which are not in CP1252 but valid in UTF8.
    /// "Привет" means "Hello" in Russian.
    /// </summary>
    private const string TestStringWithUtf8Chars = "Привет";

    [Theory, MutagenModAutoData]
    public void WithUtf8Encoding_WritesUtf8Bytes(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);

        // Create an NPC with a name containing UTF8 characters
        var npc = mod.Npcs.AddNew();
        npc.EditorID = "TestNpc";
        npc.Name = TestStringWithUtf8Chars;

        // Write the mod with UTF8 encoding enabled
        mod.BeginWrite
            .ToPath(outputPath, fileSystem)
            .WithNoLoadOrder()
            .WithUtf8Encoding()
            .Write();

        // Read the file bytes
        var fileBytes = fileSystem.File.ReadAllBytes(outputPath);

        // Convert the test string to UTF8 bytes
        var utf8Bytes = Encoding.UTF8.GetBytes(TestStringWithUtf8Chars);

        // Search for the UTF8 encoded string in the file
        var foundUtf8 = FindBytePattern(fileBytes, utf8Bytes);
        foundUtf8.ShouldBeTrue($"Expected to find UTF8 encoded bytes for '{TestStringWithUtf8Chars}' in the mod file");

        // Verify CP1252 encoding is NOT present (it would be mojibake)
        // CP1252 cannot properly encode Cyrillic, so we verify it's not trying to use CP1252
        var cp1252Bytes = Encoding.GetEncoding(1252).GetBytes(TestStringWithUtf8Chars);
        var foundCp1252 = FindBytePattern(fileBytes, cp1252Bytes);

        // If UTF8 chars can't be represented in CP1252, the bytes will be different
        // We verify that we're NOT using the CP1252 version
        if (!utf8Bytes.SequenceEqual(cp1252Bytes))
        {
            foundCp1252.ShouldBeFalse($"Should not find CP1252 encoded bytes when UTF8 encoding is enabled");
        }
    }

    [Theory, MutagenModAutoData]
    public void WithUtf8Encoding_False_WritesDefaultEncoding(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);

        // Use a string that works in both CP1252 and UTF8 but encodes differently
        const string testString = "Café"; // é has different byte representation in CP1252 vs UTF8

        var npc = mod.Npcs.AddNew();
        npc.EditorID = "TestNpc";
        npc.Name = testString;

        // Write the mod with UTF8 encoding disabled (default behavior)
        mod.BeginWrite
            .ToPath(outputPath, fileSystem)
            .WithNoLoadOrder()
            .WithUtf8Encoding(false)
            .Write();

        // Read the file bytes
        var fileBytes = fileSystem.File.ReadAllBytes(outputPath);

        // CP1252 bytes: é = 0xE9
        var cp1252Bytes = Encoding.GetEncoding(1252).GetBytes(testString);

        // UTF8 bytes: é = 0xC3 0xA9 (two bytes)
        var utf8Bytes = Encoding.UTF8.GetBytes(testString);

        // Verify they encode differently
        utf8Bytes.SequenceEqual(cp1252Bytes).ShouldBeFalse("UTF8 and CP1252 should encode 'Café' differently");

        // When UTF8 is disabled, it should use CP1252 (default)
        var foundCp1252 = FindBytePattern(fileBytes, cp1252Bytes);
        foundCp1252.ShouldBeTrue($"Expected to find CP1252 encoded bytes for '{testString}' when UTF8 is disabled");
    }

    [Theory, MutagenModAutoData]
    public void WithUtf8Encoding_ReadBack_PreservesUtf8Characters(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);

        // Create an NPC with UTF8 characters
        var originalNpc = mod.Npcs.AddNew();
        originalNpc.EditorID = "TestNpc";
        originalNpc.Name = TestStringWithUtf8Chars;
        var originalFormKey = originalNpc.FormKey;

        // Write with UTF8 encoding
        mod.BeginWrite
            .ToPath(outputPath, fileSystem)
            .WithNoLoadOrder()
            .WithUtf8Encoding()
            .Write();

        // Read the mod back with UTF8 encoding
        var loadedMod = SkyrimMod.CreateFromBinaryOverlay(
            outputPath,
            (SkyrimRelease)mod.GameRelease,
            new BinaryReadParameters()
            {
                FileSystem = fileSystem,
                StringsParam = new StringsReadParameters()
                {
                    NonLocalizedEncodingOverride = MutagenEncoding._utf8
                }
            });

        // Find the NPC
        var loadedNpc = loadedMod.Npcs.First(n => n.FormKey == originalFormKey);

        // Verify the name was preserved correctly
        loadedNpc.Name?.String.ShouldBe(TestStringWithUtf8Chars,
            "UTF8 characters should be preserved when reading with WithUtf8Encoding()");
    }

    [Theory, MutagenModAutoData]
    public void WithUtf8Encoding_RoundTrip_PreservesComplexUtf8(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);

        // Test with various UTF8 characters from different languages
        var testStrings = new[]
        {
            "日本語",           // Japanese
            "中文",             // Chinese
            "한글",             // Korean
            "العربية",          // Arabic
            "Ελληνικά",         // Greek
            "עברית",            // Hebrew
            "Emoji: 🎮🎲🗡️",   // Emoji
        };

        var createdNpcs = new Dictionary<FormKey, string>();

        foreach (var testStr in testStrings)
        {
            var npc = mod.Npcs.AddNew();
            npc.EditorID = $"TestNpc_{testStrings.ToList().IndexOf(testStr)}";
            npc.Name = testStr;
            createdNpcs[npc.FormKey] = testStr;
        }

        // Write with UTF8 encoding
        mod.BeginWrite
            .ToPath(outputPath, fileSystem)
            .WithNoLoadOrder()
            .WithUtf8Encoding()
            .Write();

        // Read back with UTF8 encoding
        var loadedMod = SkyrimMod.CreateFromBinaryOverlay(
            outputPath,
            (SkyrimRelease)mod.GameRelease,
            new BinaryReadParameters()
            {
                FileSystem = fileSystem,
                StringsParam = new StringsReadParameters()
                {
                    NonLocalizedEncodingOverride = MutagenEncoding._utf8
                }
            });

        // Verify all NPCs have correct names
        foreach (var (formKey, expectedName) in createdNpcs)
        {
            var loadedNpc = loadedMod.Npcs.FirstOrDefault(n => n.FormKey == formKey);
            loadedNpc.ShouldNotBeNull($"NPC with FormKey {formKey} should exist in loaded mod");
            loadedNpc.Name?.String.ShouldBe(expectedName,
                $"Name should be preserved for NPC with FormKey {formKey}");
        }
    }

    [Theory, MutagenModAutoData]
    public void WithUtf8Encoding_MismatchedReadWrite_DataCorruption(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);

        // Create NPC with UTF8 characters
        var npc = mod.Npcs.AddNew();
        npc.EditorID = "TestNpc";
        npc.Name = TestStringWithUtf8Chars;
        var originalFormKey = npc.FormKey;

        // Write WITH UTF8 encoding
        mod.BeginWrite
            .ToPath(outputPath, fileSystem)
            .WithNoLoadOrder()
            .WithUtf8Encoding()
            .Write();

        // Read WITHOUT UTF8 encoding (default CP1252)
        var loadedMod = SkyrimMod.CreateFromBinaryOverlay(
            outputPath,
            (SkyrimRelease)mod.GameRelease,
            new BinaryReadParameters()
            {
                FileSystem = fileSystem
                // Not setting NonLocalizedEncodingOverride = reading with default encoding
            });

        var loadedNpc = loadedMod.Npcs.First(n => n.FormKey == originalFormKey);

        // The name will be corrupted because we read UTF8 bytes as CP1252
        loadedNpc.Name?.String.ShouldNotBe(TestStringWithUtf8Chars,
            "Reading UTF8 data with CP1252 encoding should result in corrupted text");
    }

    /// <summary>
    /// Helper method to find a byte pattern within a byte array
    /// </summary>
    private static bool FindBytePattern(byte[] haystack, byte[] needle)
    {
        if (needle.Length == 0 || haystack.Length < needle.Length)
            return false;

        for (int i = 0; i <= haystack.Length - needle.Length; i++)
        {
            bool found = true;
            for (int j = 0; j < needle.Length; j++)
            {
                if (haystack[i + j] != needle[j])
                {
                    found = false;
                    break;
                }
            }
            if (found)
                return true;
        }
        return false;
    }
}
