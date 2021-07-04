using System.IO;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using DynamicData;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;
using NSubstitute;
using Xunit;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class PluginLiveLoadOrderProviderTests
    {
        [Fact]
        public async Task Integration()
        {
            using var tmpFolder = Utility.GetTempFolder(nameof(PluginLiveLoadOrderProviderTests));
            var path = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            File.WriteAllLines(path,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Update.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                });
            var live = PluginListings.GetLiveLoadOrder(GameRelease.SkyrimLE, path, default, out var state);
            var list = live.AsObservableList();
            Assert.Equal(3, list.Count);
            Assert.Equal(Skyrim.Constants.Skyrim, list.Items.ElementAt(0).ModKey);
            Assert.Equal(Skyrim.Constants.Update, list.Items.ElementAt(1).ModKey);
            Assert.Equal(Skyrim.Constants.Dawnguard, list.Items.ElementAt(2).ModKey);
            File.WriteAllLines(path,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                });
            await Task.Delay(200);
            Assert.Equal(2, list.Count);
            Assert.Equal(Skyrim.Constants.Skyrim, list.Items.ElementAt(0).ModKey);
            Assert.Equal(Skyrim.Constants.Dawnguard, list.Items.ElementAt(1).ModKey);
            File.WriteAllLines(path,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                    Skyrim.Constants.Dragonborn.ToString(),
                });
            await Task.Delay(200);
            Assert.Equal(3, list.Count);
            Assert.Equal(Skyrim.Constants.Skyrim, list.Items.ElementAt(0).ModKey);
            Assert.Equal(Skyrim.Constants.Dawnguard, list.Items.ElementAt(1).ModKey);
            Assert.Equal(Skyrim.Constants.Dragonborn, list.Items.ElementAt(2).ModKey);
        }

        [Fact]
        public void QueriesOnceInitially()
        {
            var pluginPath = "C:/SomePlugin.txt";
            var fs = Substitute.For<IFileSystem>();
            fs.FileSystemWatcher.Returns(new MockFileSystemWatcherFactory());
            var listings = Substitute.For<IPluginListingsProvider>();
            var list = new PluginLiveLoadOrderProvider(
                    fs,
                    listings,
                    new PluginListingsPathInjection(pluginPath))
                .Get(out _)
                .AsObservableList();
            list.Items.Should().BeEmpty();
            listings.Received(1).Get();
        }

        [Fact]
        public void QueriesIfChanged()
        {
            var pluginPath = "C:/SomePlugin.txt";
            var modified = new MockFileSystemWatcher();
            var fs = Substitute.For<IFileSystem>();
            fs.FileSystemWatcher.Returns(new MockFileSystemWatcherFactory(modified));
            var listings = Substitute.For<IPluginListingsProvider>();
            var list = new PluginLiveLoadOrderProvider(
                    fs,
                    listings,
                    new PluginListingsPathInjection(pluginPath))
                .Get(out _)
                .AsObservableList();
            list.Items.Should().BeEmpty();
            listings.Received(1).Get();
            
            modified.MarkChanged(pluginPath);
            listings.Received(2).Get();
        }

        [Fact]
        public void ReturnsNewListings()
        {
            var pluginPath = "C:/SomePlugin.txt";
            var modified = new MockFileSystemWatcher();
            var fs = Substitute.For<IFileSystem>();
            fs.FileSystemWatcher.Returns(new MockFileSystemWatcherFactory(modified));
            var listings = new ModListing[]
            {
                new ModListing(Utility.MasterModKey, true),
                new ModListing(Utility.MasterModKey2, false),
            };
            var listingsProv = Substitute.For<IPluginListingsProvider>();
            listingsProv.Get().Returns(listings);
            var list = new PluginLiveLoadOrderProvider(
                    fs,
                    listingsProv,
                    new PluginListingsPathInjection(pluginPath))
                .Get(out _)
                .AsObservableList();
            list.Items.Should().Equal(listings);
            
            var listings2 = new ModListing[]
            {
                new ModListing(Utility.MasterModKey, true),
                new ModListing(Utility.MasterModKey2, false),
                new ModListing(Utility.MasterModKey3, true),
            };
            listingsProv.Get().Returns(listings2);
            modified.MarkChanged(pluginPath);
            list.Items.Should().Equal(listings2);
        }

        [Fact]
        public void NoErrors()
        {
            var pluginPath = "C:/SomePlugin.txt";
            var modified = new MockFileSystemWatcher();
            var fs = Substitute.For<IFileSystem>();
            fs.FileSystemWatcher.Returns(new MockFileSystemWatcherFactory(modified));
            var listings = Substitute.For<IPluginListingsProvider>();
            new PluginLiveLoadOrderProvider(
                    fs,
                    listings,
                    new PluginListingsPathInjection(pluginPath))
                .Get(out var state)
                .AsObservableList();
            TestScheduler scheduler = new TestScheduler();
            var testableObs = scheduler.CreateObserver<ErrorResponse>();
            using var sub = state.Subscribe(testableObs);
            testableObs.Messages.Should().HaveCount(1);
            testableObs.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObs.Messages[0].Value.Value.Succeeded.Should().BeTrue();
            modified.MarkChanged(pluginPath);
            testableObs.Messages.Should().HaveCount(2);
            testableObs.Messages[1].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObs.Messages[1].Value.Value.Succeeded.Should().BeTrue();
        }

        [Fact]
        public void ErrorFlop()
        {
            var pluginPath = "C:/SomePlugin.txt";
            TestScheduler scheduler = new();
            var modified = new MockFileSystemWatcher();
            var fs = Substitute.For<IFileSystem>();
            fs.FileSystemWatcher.Returns(new MockFileSystemWatcherFactory(modified));
            var listings = new ModListing[]
            {
                new ModListing(Utility.MasterModKey, true),
                new ModListing(Utility.MasterModKey2, false),
            };
            var listingsProv = A.Fake<IPluginListingsProvider>();
            A.CallTo(() => listingsProv.Get()).Returns(listings);
            var list = new PluginLiveLoadOrderProvider(
                    fs,
                    listingsProv,
                    new PluginListingsPathInjection(pluginPath))
                .Get(out var state)
                .AsObservableList();
            list.Count.Should().Be(2);
            var testableObs = scheduler.CreateObserver<ErrorResponse>();
            using var sub = state.Subscribe(testableObs);
            testableObs.Messages.Should().HaveCount(1);
            testableObs.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObs.Messages[0].Value.Value.Succeeded.Should().BeTrue();

            A.CallTo(() => listingsProv.Get()).Throws(() => new NotImplementedException());
            modified.MarkChanged(pluginPath);
            testableObs.Messages.Should().HaveCount(2);
            testableObs.Messages[1].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObs.Messages[1].Value.Value.Succeeded.Should().BeFalse();

            A.CallTo(() => listingsProv.Get()).Returns(listings);
            modified.MarkChanged(pluginPath);
            list.Count.Should().Be(2);
            testableObs.Messages.Should().HaveCount(3);
            testableObs.Messages[2].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObs.Messages[2].Value.Value.Succeeded.Should().BeTrue();
        }

        [Fact]
        public void Changed()
        {
            var pluginPath = "C:/SomePlugin.txt";
            TestScheduler scheduler = new();
            var modified = new MockFileSystemWatcher();
            var fs = Substitute.For<IFileSystem>();
            fs.FileSystemWatcher.Returns(new MockFileSystemWatcherFactory(modified));
            var testableObs = scheduler.CreateObserver<Unit>();
            new PluginLiveLoadOrderProvider(
                    fs,
                    Substitute.For<IPluginListingsProvider>(),
                    new PluginListingsPathInjection(pluginPath))
                .Changed
                .Subscribe(testableObs);
            testableObs.ShouldNotBeCompleted();
            testableObs.ShouldHaveNoErrors();
            testableObs.Messages.Should().HaveCount(1);
            
            modified.MarkChanged(pluginPath);
            testableObs.ShouldNotBeCompleted();
            testableObs.ShouldHaveNoErrors();
            testableObs.Messages.Should().HaveCount(2);
        }
    }
}